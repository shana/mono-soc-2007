/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 17.7.2007
 * Time: 21:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace NMS
{
	/// <summary>
	/// Description of ITransactionContext.
	/// </summary>
	public interface ITransactionContext
	{
		
        /// <summary>
		/// Return current transaction id
		/// </summary>
        string Id
        {
            get;
        }      
        
        /// <summary>
		/// Begin session transaction
		/// </summary>        
        void Begin();
        
        /// <summary>
		/// Rollback current session transaction
		/// </summary>              
        void Rollback();
		
        /// <summary>
		/// Commit current session transaction
		/// </summary>        
        void Commit();
	}
}
