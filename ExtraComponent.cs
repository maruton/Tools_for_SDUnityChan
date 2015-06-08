/*
 *	@file		ExtraComponent.cs
 *	@note		
 *	@attention	
 *				[ExtraComponent.cs]
 *				Copyright (c) [2015] [Maruton]
 *				This software is released under the MIT License.
 *				http://opensource.org/licenses/mit-license.php
 */
using UnityEngine;
using System.Collections;
using System.Linq; // for LINQ/Lambda
using System.Collections.Generic;//!< for List

public static class ExtraComponent {
	/*!	コンポーネント<T>を持つ子オブジェクトの一覧を作成して返す 
	 * 	コンポーネント<T>を持つ子オブジェクトの一覧を作成して返す 
	 * 	@return		該当した<T>のArrayを返す。 
	 * 	@retval		null		該当なし 
   	 * 	@note		多階層を含む全ての子オブジェクトが一覧作成対象 
   	 * 	@attention	
   	 */
	public static T[] GetComponentsInChildrenWithoutSelf<T>(this GameObject self) where T : Component{
		return self.GetComponentsInChildren<T>().Where(c => self != c.gameObject).ToArray();
	}

	/*!	コンポーネント<T>を持つ子オブジェクト中から指定Object名のものを検索して返す 
	 * 	コンポーネント<T>を持つ子オブジェクト中から指定Object名のものを検索して返す 
	 * 	@return		該当した<T>を返す。 
	 * 	@retval		非null		該当した<T>を返す 
	 * 	@retval		null		該当なし、又は複数該当した場合 
   	 * 	@note		
   	 * 	@attention	多階層を含む全ての子オブジェクトを対象にコンポーネント、Object名検索する 
   	 */
	public static T FindComponent_of_ChildHierarchy<T>(this GameObject self, string findName) where T : Component{
		T[] T_Target = self.GetComponentsInChildrenWithoutSelf<T>();
		var q = T_Target.Where(n => n.name == findName ); 
		if(q.Count()!=0){
			T result = (q.ToArray())[0];
			return(result);
		}
		return(null);
	}
	/*!	FindComponent_of_ChildHierarchy のエラーコード対応 
	 * 	FindComponent_of_ChildHierarchy のエラーコード対応 
	 * 	@param [in,out]		retCode		0:該当なし 1以上:該当個数 
	 * 	@return				該当した<T>を返す。 
	 * 	@retval				非null		該当した<T>を返す 
	 * 	@retval				null		該当なし、又は複数該当した場合 
   	 * 	@note		
   	 * 	@attention	多階層を含む全ての子オブジェクトを対象にコンポーネント、Object名検索する 
   	 */
	public static T FindComponent_of_ChildHierarchy<T>(this GameObject self, string findName, out int retCode) where T : Component{
		T[] T_Target = self.GetComponentsInChildrenWithoutSelf<T>();
		var q = T_Target.Where(n => n.name == findName ); 
		if((retCode=q.Count())!=0){
			T result = (q.ToArray())[0];
			return(result);
		}
		return(null);
	}
}
