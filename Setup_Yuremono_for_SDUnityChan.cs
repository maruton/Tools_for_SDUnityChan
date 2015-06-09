/*
 *	@file		Setup_Yuremono_for_SDUnityChan.cs
 *	@note		FBXインポートしたSDユニティちゃんアバター（ユレモノ未設定）に
 *				自動でユレモノ設定を行う。 
 *	@attention	Blogger記事 http://dennou-note.blogspot.jp/2015/05/sd-blender.html にて
 *				エクスポートしたアバターモデルを自動設定対象にしています。  
 *				※ボーン構造やローカル軸の向きが異なるとうまく動作しない場合がありえます。 
 *				このスクリプトを使う際は予め公式配布のゆれもの設定済みＳＤユニティちゃんを同シーンに配置する必要があります。 
 *				コメント欄などでは自動設定対象アバターモデルを Blender版 と表記している場合があります。 
 *				
 *				[Setup_Yuremono_for_SDUnityChan.cs]
 *				Copyright (c) [2015] [Maruton]
 *				This software is released under the MIT License.
 *				http://opensource.org/licenses/mit-license.php
 */
using UnityEngine;
using System.Collections;
using System.Linq; // for LINQ/Lambda
using System.Collections.Generic;//!< for List
using System;//!< for Array.Resize

namespace UnityChan{
	public class Setup_Yuremono_for_SDUnityChan : MonoBehaviour {
		string MyScriptName = "[Setup_Yuremono_for_SDUnityChan]";//!< Use report to debug message.

		public GameObject go_Original, go_Target;
		Vector2 go_Target_Position; //!< 元のGlobal Positionを格納 
		
		public int Cnt_Original;//!< Original内Object総数 
		public int Cnt_Target;	//!< Target内Object総数 

		//確認用のカウンタ類
		public int Cnt_Diff; 				//!< Originalにのみ存在するObjectの個数（Targetで不足しているObject） 
		public int Cnt_Added_Object;		//!< Targetに追加されたObjectの個数 
		public int Cnt_Added_SpringCollider;//!< Targetに追加されたスクリプトSpringColliderの個数 
		public int Cnt_Added_SpringBone;	//!< Targetに追加されたスクリプトSpringBoneの個数 

		public List<Transform> tr_Diff = new List<Transform>();//!< Originalのみ存在するObjectのリスト（Targetで不足しているObject） 

		string Original_DefautName = "SD_unitychan_humanoid";	//!< 処理対象とするOriginal SDユニティちゃんのデフォルト値
		string Target_DefaultName = "blender_SDUnityChan";		//!< 処理対象とするBlender版 SDユニティちゃんのデフォルト値
		Transform[] tr_Original;	//!< Originalの持つ全Object transform の一覧 
		Transform[] tr_Target;		//!< Targetの持つ全Object transform の一覧 
		SpringManager cs_target_SpringManager; //!< Target の SpringManager を参照(root object内のコンポーネント) 


		/*!	エラー判定及びエラーメッセージ表示を行う 
		 *	エラー判定及びエラーメッセージ表示を行う  
		 *	@param [out]		true: エラーあり  false:エラーなし
    	 * 	@note		 
    	 * 	@attention
    	 */
		bool Checker_with_ErrorMessage(int n, string s){
			if(n==0){// Not found
				Debug.Log (MyScriptName+" ERROR: Not found '"+s+"'.");
				return(true);
			}
			else if(n>1){//multiple found
				Debug.Log (MyScriptName+" ERROR: Multiple found '"+s+"'.");
				return(true);
			}
			return(false);
		}

		/*!	ユレモノの向きのデータ作成(BoneAxis) 
		 *	ユレモノの向きのデータ作成(BoneAxis) 
    	 * 	@note		go_Original: OriginalのGameobjet格納 
    	 * 	@attention	Blog記事でBlenderでインポート＆エクスポートしたアバターに合わせ込まれています。 
    	 * 				他の方法で作成したアバターはオリジナルと比較し再調整が必要になる可能性があります。 
    	 */
		Dictionary<string, Vector3> customBoneAxis = new Dictionary<string, Vector3>();
		void Make_dic_for_BoneAxis(){
			customBoneAxis.Add("J_L_HairFront_00", new Vector3( 0,-1,0 ));
			customBoneAxis.Add("J_L_HairSide2_00", new Vector3( 0,-1,0 ));
			customBoneAxis.Add("J_L_HairSide_00", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_L_HairSide_01", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_L_HairTail_00", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_L_HairTail_01", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_L_HairTail_02", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_L_HeadRibbon_00", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_L_HeadRibbon_01", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_L_HeadRibbon_02", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_R_HairFront_00", new Vector3( 0,-1,0 ));
			customBoneAxis.Add("J_R_HairSide2_00", new Vector3( 0,-1,0 ));
			customBoneAxis.Add("J_R_HairSide_00", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_R_HairSide_01", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_R_HairTail_00", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_R_HairTail_01", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_R_HairTail_02", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_R_HeadRibbon_00", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_R_HeadRibbon_01", new Vector3( 0,1,1 ));
			customBoneAxis.Add("J_R_HeadRibbon_02", new Vector3( 0,1,1 ));
			customBoneAxis.Add("J_acce_00", new Vector3( 0,-1,0 ));
			customBoneAxis.Add("J_L_Skirt_00", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_L_Skirt_01", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_L_SkirtBack_01", new Vector3( 0,-1,0 ));
			customBoneAxis.Add("J_R_Skirt_00", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_R_Skirt_01", new Vector3( 0,1,0 ));
			customBoneAxis.Add("J_R_SkirtBack_01", new Vector3( 0,-1,0 ));
		}

		/*!	SpringManager を root object に追加する 
		 * 	SpringManager を root object に追加する 
    	 * 	@note		
    	 * 	@attention		SpringBone は Awake()で SpringManager の存在するObjectを参照確認して保存する。 
    	 * 					その為以後 SpringBone を追加してゆく為には事前に SpringManager を存在させる必要がある。 
    	 * 					ただし SpringManager は自分自身の持つ配列 SpringBoneの要素が0だと正しく動作しない。 
    	 * 					よって SpringManager は追加はするが、動作準備が整うまでは Running させない。 
    	 */
		void Stage_Initial_SpringManager(){
			cs_target_SpringManager = go_Target.AddComponent<SpringManager>();
			cs_target_SpringManager.enabled = false;//Temporary halt cause not setting up other yet.
		}

		/*!	初期化 
		 * 	OriginalとTargetの存在確認、各々Objectのtransform一覧生成 
    	 * 	@note		go_Original: Original の Gameobjet格納 
    	 * 				go_Target:   Target の Gameobjet格納 
    	 * 				tr_Original: Original の transform一覧リスト 
    	 * 				tr_Target:   Target の transform一覧リスト 
    	 * 	@attention
    	 */
		bool Stage_Initial(){
			Cnt_Diff = Cnt_Added_Object = Cnt_Added_SpringCollider = 0;

			if(go_Original==null){
				go_Original = GameObject.Find(Original_DefautName);
				if(go_Original==null){ Debug.Log (MyScriptName+" ERROR: Not found '"+Original_DefautName+"'"); return(true); }
			}
			if(go_Target==null){
				go_Target = GameObject.Find(Target_DefaultName);
				if(go_Target==null){ Debug.Log (MyScriptName+" ERROR: Not found '"+Target_DefaultName+"'"); return(true); }
			}
			tr_Original = go_Original.GetComponentsInChildrenWithoutSelf<Transform>();
			tr_Target = go_Target.GetComponentsInChildrenWithoutSelf<Transform>();

			//Set same position
			go_Target_Position = go_Target.transform.position;
			go_Target.transform.position = go_Original.transform.position;

			//Add SpringManager
			Stage_Initial_SpringManager();

			return(false);
		}

		/*!	OriginalとTargetのObject差異一覧生成 
		 *	Originalに存在しTargetに存在しないGameObject.Transformの一覧を作成し保存する。  
		 *	@param [out]		true: エラーあり  false:エラーなし
    	 * 	@note		差異保存先 tr_Diff
    	 * 	@attention	
    	 */
		bool Stage_Make_DiffrentObjectList(){
			for(int i=0; i<tr_Original.Length; i++){
				var q = tr_Target.Where(n => n.gameObject.name == tr_Original[i].name );
				if(q.Count()==0){//not found
					tr_Diff.Add(tr_Original[i]);
				}
				else if(q.Count()>1){//multiple found
					Debug.Log (MyScriptName+" ERROR: Multiple found '"+tr_Original[i].name +"'.");
					return(true);
				}
			}
			Cnt_Original = tr_Original.Length;
			Cnt_Target = tr_Target.Length;
			Cnt_Diff = tr_Diff.Count;
			return(false);			
		}

		/*!	Target不足分の差異ObjectをTargetに追加する 
		 *	Target不足分の差異ObjectをTargetに追加する 
		 *	@param [out]		true: エラーあり  false:エラーなし
    	 * 	@note		追加するObjectの親もまた追加が必要なケースがある。 
    	 * 				この際は先に親を追加しないと子の追加ができない為その処理も行っている。 
    	 * 	@attention	スクリプト類は相互にPublic変数でGameObjectを参照している為、ここではまだ追加不可。 
    	 */
		bool Stage_Add_Object(){
			int OverCnt = 0;
			for(int i=0; i<tr_Diff.Count; i++){
				string originalParentName = tr_Diff[i].parent.name;// 親の名前取得  
				string originalCurrentName = tr_Diff[i].name;
				if(originalCurrentName!="Main Camera"){//カメラは対象外とする 
					var q = tr_Target.Where(n => n.gameObject.name == originalParentName );// 親の存在を確認する 
					if(q.Count()==0){// 親は存在しない? 
						if(originalParentName!=Original_DefautName){//not root?
							//Debug.Log ("[NOT FOUND] Parent object '"+originalParentName +"', CURRENT='"+originalCurrentName+"'");
							// この親Objectがまだ追加されていない為、このObjectはリストの最後尾に移動させ後回しにする  
							tr_Diff.Add(tr_Diff[i]);
							tr_Diff.Remove(tr_Diff[i]);
							i--;
						}
					}
					else if(q.Count()>1){//multiple found
						Debug.Log (MyScriptName+" ERROR: Multiple found '"+originalParentName +"'.");
						return(true);
					}
					else{//Found
						GameObject newChild = new GameObject(originalCurrentName);
						Transform parentTarget = (q.ToArray())[0];
						newChild.transform.parent = parentTarget;// Set parent

						newChild.transform.localRotation = tr_Diff[i].transform.localRotation;
						newChild.transform.localScale = tr_Diff[i].transform.localScale;
						newChild.transform.position = tr_Diff[i].transform.position;//Global positionでCopy(Originalと重ねてScene配置要) 

						Cnt_Added_Object++;
						//Debug.Log ("[ADDED OBJECT] '"+originalCurrentName+"' < (parent)"+originalParentName);
						
					}
					if(OverCnt++>tr_Original.Length) i = tr_Diff.Count;//Safe code
				}
			}
			tr_Target = go_Target.GetComponentsInChildrenWithoutSelf<Transform>();//Update tr_Target 
			return(false);
		}

		/*!	originalと比較しTargetに不足しているスクリプト SpringCollider を追加する 
		 * 	originalと比較しTargetに不足しているスクリプト SpringCollider を追加する 
		 *	@param [out]		true: エラーあり  false:エラーなし
    	 * 	@note		
    	 * 	@attention	SpringBone の参照先に SpringCollider がある為、
    	 * 				SpringBone よりも SpringCollider を先に追加しなければならない。  
    	 */
		bool Stage_Add_SpringCollider(){
			SpringCollider[] cs_SpringCollider = go_Original.GetComponentsInChildrenWithoutSelf<SpringCollider>();
			int OverCnt = 0;
			for(int i=0; i<cs_SpringCollider.Length; i++){
				SpringCollider current = cs_SpringCollider[i];
				var q = tr_Target.Where(n => n.gameObject.name == current.name );// is there SpringCollider? 
				if( Checker_with_ErrorMessage( q.Count(), current.name) ) return(true);
				Transform target = (q.ToArray())[0];
				SpringCollider cs_targetCurrent =  target.gameObject.AddComponent<SpringCollider>();
				cs_targetCurrent.radius = current.radius;
				//Debug.Log ("[ADDED SpringCollider] '"+target.name);
				Cnt_Added_SpringCollider++;
				if(OverCnt++>tr_Original.Length) i = tr_Diff.Count;////Safe exit code when use to broken avatar structure
			}
			return(false);
		}

		/*!	originalと比較しTargetに不足しているスクリプト SpringBone を追加する 
		 * 	originalと比較しTargetに不足しているスクリプト SpringBone を追加する 
		 *	@param [out]		true: エラーあり  false:エラーなし
    	 * 	@note		
    	 * 	@attention	SpringBone は 他のSpringCollider を参照する 、
    	 */
		bool Stage_Add_SpringBone(){
			SpringBone[] cs_original_SpringBone = go_Original.GetComponentsInChildrenWithoutSelf<SpringBone>();//!< OriginalのSpringBone一覧 
			//Add SpringBone script
			int OverCnt = 0;
			for(int i=0; i<cs_original_SpringBone.Length; i++){
				SpringBone cs_original_SpringBone_current = cs_original_SpringBone[i];
				var q = tr_Target.Where(n => n.gameObject.name == cs_original_SpringBone_current.name );// is there SpringBone? 
				if( Checker_with_ErrorMessage( q.Count(), cs_original_SpringBone_current.name) ) return(true);
				
				Transform target = (q.ToArray())[0];
				SpringBone cs_target_SpringBone = target.gameObject.AddComponent<SpringBone>();
				//target_SpringBone.enabled = false;//Temporary halt 
				
				//Begin: Set child pointer
				string script_var_child = cs_original_SpringBone_current.child.name;
				var q1 = tr_Target.Where(n => n.gameObject.name == script_var_child );
				if( Checker_with_ErrorMessage( q1.Count(), script_var_child) ) return(true);
				cs_target_SpringBone.child = (q1.ToArray())[0];//Copy var Child
				//End:   Set child pointer 

				//Begin: Set bone axis(custom data) for customized my export blender model.
				cs_target_SpringBone.boneAxis = customBoneAxis[cs_target_SpringBone.name];//(was try math calculation all, and fail... Decided to use lookup table data)
				//End:	 Set bone axis(custom data) for customized my export blender model

				//Begin: Copy other value
				cs_target_SpringBone.radius = cs_original_SpringBone_current.radius;
				cs_target_SpringBone.isUseEachBoneForceSettings = cs_original_SpringBone_current.isUseEachBoneForceSettings;
				cs_target_SpringBone.dragForce =  cs_original_SpringBone_current.dragForce;
				cs_target_SpringBone.springForce = cs_original_SpringBone_current.springForce;
				cs_target_SpringBone.debug = cs_original_SpringBone_current.debug;
				cs_target_SpringBone.threshold = cs_original_SpringBone_current.threshold;
				//End: Copy other value

				int max = cs_original_SpringBone_current.colliders.Length; //colliders array length(original avatar object's SpringBone)
				cs_target_SpringBone.colliders = new SpringCollider[0]; //initial empty array.
				for(int j=0; j<max; j++){
					string elemtntName = cs_original_SpringBone_current.colliders[j].name; //get original colliders array element
					var q2 = tr_Target.Where(n => n.gameObject.name == elemtntName );//is there (SpringCollider)element in target? 
					if( Checker_with_ErrorMessage( q2.Count(), elemtntName) ) return(true);//check to fail
					SpringCollider cs_element_SpringCollider = (q2.ToArray())[0].GetComponent<SpringCollider>();//get target collider that element
					if(cs_element_SpringCollider==null){
						Debug.Log(MyScriptName+" ERROR: Not found SpringCollider in '"+(q2.ToArray())[0]+"'");
						return(true);
					}
					Array.Resize(ref (cs_target_SpringBone.colliders), cs_target_SpringBone.colliders.Length+1);//Resize array(must be step by step so increase)
					cs_target_SpringBone.colliders[j] = cs_element_SpringCollider;//set collider element that same original structure
				}
				//Debug.Log ("[ADDED SpringBone] '"+target.name);
				Cnt_Added_SpringBone++;
				if(OverCnt++>tr_Original.Length) i = tr_Diff.Count;//Safe exit code when use to broken avatar structure
			}
			return(false);
		}


		/*!	Target の SpringManager の変数類を originalからコピーする。  
		 *	Target の SpringManager の変数類を originalからコピーする。	
		 *	@param [out]		true: エラーあり  false:エラーなし
    	 * 	@note		
    	 * 	@attention	
    	 */
		bool Stage_Add_SpringManager(){
			SpringManager cs_original_SpringManager = go_Original.GetComponent<SpringManager>();
			//cs_target_SpringManager.enabled = false;//Temporary halt 

			//Begin: Copy value
			cs_target_SpringManager.dynamicRatio = cs_original_SpringManager.dynamicRatio;
			cs_target_SpringManager.stiffnessForce = cs_original_SpringManager.stiffnessForce;
			cs_target_SpringManager.dragForce = cs_original_SpringManager.dragForce;
			//End: Copy value

			//Begin: Copy AnimationCurve
			cs_target_SpringManager.stiffnessCurve = new AnimationCurve();
			for(int i=0; i<cs_original_SpringManager.stiffnessCurve.keys.Length; i++){
				cs_target_SpringManager.stiffnessCurve.AddKey( cs_original_SpringManager.stiffnessCurve.keys[i] );
			}
			cs_target_SpringManager.dragCurve = new AnimationCurve();
			for(int i=0; i<cs_original_SpringManager.dragCurve.keys.Length; i++){
				cs_target_SpringManager.dragCurve.AddKey( cs_original_SpringManager.dragCurve.keys[i] );
			}
			//End: Copy AnimationCurve

			//Begin: Set element of array(SpringBone).
			cs_target_SpringManager.springBones = new SpringBone[0];//initial empty array.
			for(int i=0; i<cs_original_SpringManager.springBones.Length; i++){
				string elemtntName = cs_original_SpringManager.springBones[i].name;
				var q = tr_Target.Where(n => n.gameObject.name == elemtntName );//find element in target  
				if( Checker_with_ErrorMessage( q.Count(), elemtntName) ) return(true);
				SpringBone cs_element_SpringBone = (q.ToArray())[0].GetComponent<SpringBone>();
				if(cs_element_SpringBone==null){
					Debug.Log(MyScriptName+" ERROR: Not found SpringCollider in '"+(q.ToArray())[0]+"'");
					return(true);
				}
				Array.Resize(ref (cs_target_SpringManager.springBones), cs_target_SpringManager.springBones.Length+1);//Resize array
				cs_target_SpringManager.springBones[i] = cs_element_SpringBone;
			}
			//End: Set element of array(SpringBone).

			//Debug.Log ("[ADDED SpringBone] '"+cs_target_SpringManager.name);
			return(false);
		}

		/*!	ユレモノ付与処理完了後の追加処理
		 *	ターゲットを元の位置へ戻し、SpringManegerの稼働を開始する（ユレモノ動作開始）。 
    	 * 	@note		
    	 * 	@attention	
    	 */
		void Stage_PostInitial(){
			go_Target.transform.position = go_Target_Position;
			//cs_target_SpringManager.enabled = true; //SpringManager go exec.
		}

		/*!	Called when first frame.
		 *	Called when first frame.
    	 * 	@note		
    	 * 	@attention	
    	 */
		void Start () {
			Debug.Log (MyScriptName+" START:");
			Make_dic_for_BoneAxis();// Make BoneAxis data.
			if( Stage_Initial() ) return;
			if( Stage_Make_DiffrentObjectList() ) return;
			if( Stage_Add_Object() ) return;
			if( Stage_Add_SpringCollider() ) return;
			if( Stage_Add_SpringBone() ) return;
			if( Stage_Add_SpringManager() ) return;
			Stage_PostInitial();
			Debug.Log (MyScriptName+" COMPLETE:");
		}


		//void Update () {
		//
		//}
	}
}
