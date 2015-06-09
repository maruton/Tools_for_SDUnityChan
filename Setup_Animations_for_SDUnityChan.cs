/*
 *	@file		Setup_Animations_for_SDUnityChan.cs
 *	@note		自前でFBXインポートしたSDユニティちゃんモデルに
 *				指定の公式配布モデルと同じようにアニメーション関連の
 *				スクリプト、アニメーションコントローラを設定する。
 *				設定概要：
 *					Animatorへアニメーションコントローラ設定
 *					IdleChangerスクリプト追加、パラメータ設定
 *					FaceUpdateスクリプト追加、パラメータ設定(表情アニメ等)
 *					AutoBlinkforSDスクリプト追加、パラメータ設定(faceメッシュ指定等)
 *					RandomWindスクリプト追加、パラメータ設定
 *	@attention	
 *				[Setup_Animations_for_SDUnityChan.cs]
 *				Copyright (c) [2015] [Maruton]
 *				This software is released under the MIT License.
 *				http://opensource.org/licenses/mit-license.php
 */
using UnityEngine;
using System.Collections;
using System.Linq; // for LINQ/Lambda
using System;//!< for Array.Resize

namespace UnityChan{
	public class Setup_Animations_for_SDUnityChan : MonoBehaviour {
		string MyScriptName = "[Setup_Animations_for_SDUnityChan]";//!< Use report to debug message.
		string Source_Name = "SD_unitychan_humanoid";	//!< Default source avatar model name.
		string Target_Name = "blender_SDUnityChan";		//!< Default target avatar model name.
		public GameObject go_Source;	//!< Source avatar model.
		public GameObject go_Target;	//!< Target avatar model.

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

		/*!	初期化 
		 * 	go_Sourceとgo_Target存在確認＆初期化 
		 * 	@return		エラーの有無
		 * 	@retval		true	エラー有
		 * 	@retval		false	エラー無
    	 * 	@note		go_Sourceがエディタで未指定の場合は Source_Name をシーンから検索して設定する。
    	 * 				みつからない場合はエラーを返す。
    	 * 				go_Target_Nameがエディタで未指定の場合は Target_Name をシーンから検索して設定する。
    	 * 				みつからない場合はエラーを返す。
    	 * 	@attention
    	 */
		bool initial(){
			if(go_Source==null){
				if ((go_Source = GameObject.Find(Source_Name))==null){
					Debug.Log(MyScriptName+" ERROR: Please set source avatar.");
					return(true);
				}
			}
			
			if(go_Target==null){
				if((go_Target = GameObject.Find(Target_Name))==null){
					Debug.Log(MyScriptName+" ERROR: Please Set target avatar.");
					return(true);
				}
			}
			return(false);
		}

		/*!	Animatorの設定 
		 * 	Controllerの設定をする
		 * 	@return		エラーの有無
		 * 	@retval		true	エラー有
		 * 	@retval		false	エラー無
    	 * 	@note		Animator の controllerを go_Source から go_Targetへコピーする。 
    	 * 	@attention	ただしAnimatorはDisableとする。 Assetに拾った際にデフォルトのポーズのまま取得したい為。 
    	 */
		bool Setup_Animator(){
			Animator animator_src;
			Animator animator_dst;
			animator_src = go_Source.GetComponent<Animator>();
			animator_dst = go_Target.GetComponent<Animator>();
			animator_dst.enabled = false;//Disable Animator.
			animator_dst.runtimeAnimatorController = animator_src.runtimeAnimatorController;
			return(false);
		}

		/*!	IdleChanger の追加＆設定 
		 * 	IdleChanger の追加＆設定 
		 * 	@return		エラーの有無
		 * 	@retval		true	エラー有
		 * 	@retval		false	エラー無
    	 * 	@note		IdleChanger がなければ追加する。
    	 * 				IdleChanger の パラメータ値を go_Source から go_Target へ コピーする。 
    	 * 	@attention
    	 */
		bool Setup_IdleChanger(){
			IdleChanger cs_dst;
			IdleChanger cs_src;
			
			if((cs_src = go_Source.GetComponent<IdleChanger>())==null){
				Debug.Log(MyScriptName+" ERROR: Not found script 'IdleChanger' in "+go_Source.name);
				return(true);
			}
			if((cs_dst = go_Target.GetComponent<IdleChanger>())==null){
				cs_dst = go_Target.AddComponent<IdleChanger>();
			}
			cs_dst._random = cs_src._random;
			cs_dst._threshold = cs_src._threshold;
			cs_dst._interval = cs_src._interval;
			return(false);
		}
		
		/*!	FaceUpdate の追加＆設定 
		 * 	FaceUpdate の追加＆設定 
		 * 	@return		エラーの有無
		 * 	@retval		true	エラー有
		 * 	@retval		false	エラー無
    	 * 	@note		FaceUpdate がなければ追加する。
    	 * 				FaceUpdate の パラメータ値を go_Source から go_Target へ コピーする。 
    	 * 	@attention
    	 */
		bool Setup_FaceUpdate(){
			FaceUpdate cs_src;
			FaceUpdate cs_dst;

			if((cs_src = go_Source.GetComponent<FaceUpdate>())==null){
				Debug.Log(MyScriptName+" ERROR: Not found script 'FaceUpdate' in "+go_Source.name);
				return(true);
			}
			if((cs_dst = go_Target.GetComponent<FaceUpdate>())==null){
				cs_dst = go_Target.AddComponent<FaceUpdate>();
			}
			cs_dst.animations = new AnimationClip[0]; //initial empty array.
			for(int i=0; i<cs_src.animations.Length; i++){
				Array.Resize(ref (cs_dst.animations), cs_dst.animations.Length + 1);
				cs_dst.animations[i] = cs_src.animations[i];
			}
			cs_dst.delayWeight = cs_src.delayWeight;
			cs_dst.isKeepFace = cs_src.isKeepFace;
			return(false);
		}

		/*!	AutoBlinkforSD の追加＆設定 
		 * 	AutoBlinkforSD の追加＆設定 
		 * 	@return		エラーの有無
		 * 	@retval		true	エラー有
		 * 	@retval		false	エラー無
    	 * 	@note		AutoBlinkforSD がなければ追加する。
    	 * 				AutoBlinkforSD はDisableとする。 
    	 * 				AutoBlinkforSD の パラメータ値を go_Source から go_Target へ コピーする。 
    	 * 	@attention	ただしDisableとする。 Assetに拾った際にデフォルトのポーズのまま取得したい為。 
    	 */
		bool Setup_AutoBlinkforSD(){
			string refName = "_face";
			AutoBlinkforSD cs_dst;
			AutoBlinkforSD cs_src;
			
			if((cs_src = go_Source.GetComponent<AutoBlinkforSD>())==null){
				Debug.Log(MyScriptName+" ERROR: Not found script 'AutoBlinkforSD' in "+go_Source.name);
				return(true);
			}
			if((cs_dst = go_Target.GetComponent<AutoBlinkforSD>())==null){
				cs_dst = go_Target.AddComponent<AutoBlinkforSD>();
			}
			cs_dst.enabled = false;//Disable script running.
			
			cs_dst.isActive = cs_src.isActive;

			cs_dst.ratio_Close = cs_src.ratio_Close;
			cs_dst.ratio_HalfClose = cs_src.ratio_HalfClose;

			int retCode;
			SkinnedMeshRenderer q = go_Target.FindComponent_of_ChildHierarchy<SkinnedMeshRenderer>(refName, out retCode);
			if( Checker_with_ErrorMessage( retCode, refName+"(SkinnedMeshRenderer)" ) ) return(true);
			cs_dst.ref_face = q;

			cs_dst.index_EYE_blk = cs_src.index_EYE_blk;
			cs_dst.index_EYE_sml = cs_src.index_EYE_sml;
			cs_dst.index_EYE_dmg = cs_src.index_EYE_dmg;
			cs_dst.timeBlink = cs_src.timeBlink;
			cs_dst.threshold = cs_src.threshold;
			cs_dst.interval = cs_src.interval;
			return(false);
		}

		/*!	AutoBlinkforSD の追加＆設定 
		 * 	AutoBlinkforSD の追加＆設定 
		 * 	@return		エラーの有無
		 * 	@retval		true	エラー有
		 * 	@retval		false	エラー無
    	 * 	@note		AutoBlinkforSD がなければ追加する。
    	 * 				AutoBlinkforSD はDisableとする。 
    	 * 				AutoBlinkforSD の パラメータ値を go_Source から go_Target へ コピーする。 
    	 * 	@attention	ただしDisableとする。 Assetに拾った際にデフォルトのポーズのまま取得したい為。 
    	 */
		bool Setup_RandomWind(){
			RandomWind cs_dst;
			RandomWind cs_src;
			
			if((cs_src = go_Source.GetComponent<RandomWind>())==null){
				Debug.Log(MyScriptName+" ERROR: Not found script 'RandomWind' in "+go_Source.name);
				return(true);
			}
			if((cs_dst = go_Target.GetComponent<RandomWind>())==null){
				cs_dst = go_Target.AddComponent<RandomWind>();
			}
			cs_dst.enabled = false;//Disalbe script running.
			
			cs_dst.isWindActive = cs_src.isWindActive;
			cs_dst.threshold = cs_src.threshold;
			cs_dst.interval = cs_src.interval;
			cs_dst.windPower = cs_src.windPower;
			cs_dst.gravity = cs_src.gravity;
			return(false);
		}

		/*!	Call when first frame.
		 * 	Call when first frame.
    	 * 	@note
    	 * 	@attention
		 */
		void Start () {
			Debug.Log (MyScriptName+" START:");
			bool Result;
			if( initial() ) return;
			if( Setup_Animator() ) return;
			if( Setup_IdleChanger() ) return;
			if( Setup_FaceUpdate() ) return;
			if( Setup_AutoBlinkforSD() ) return;
			if( Setup_RandomWind() ) return;
			Debug.Log (MyScriptName+" COMPLETE:");
		}
	}
}
