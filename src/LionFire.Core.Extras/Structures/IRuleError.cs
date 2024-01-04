using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{


    ///// <summary>
    ///// An error about a game condition, for example in a theatre that is missing some elements in its map, or conflicting rules.
    ///// </summary>
    //public interface IRuleError
    //{

    //    /// <summary>
    //    /// A short one or two word description of the problem or problem area
    //    /// </summary>
    //    string Category { get; }

    //    /// <summary>
    //    /// A concise description of the error and how to fix it.
    //    /// </summary>
    //    string Message { get; }

    //    /// <summary>
    //    /// Detailed secondary info about the error.
    //    /// </summary>
    //    string Remarks { get; }

    //    string SuggestedFix { get; }


    //    //#region AutoFix

    //    //IList<string> FixDescriptions { get; }
    //    //bool TryFix(int fixIndex);

    //    //#endregion
        
    //}

    /// <summary>
    /// Deprecated: Use ValidationIssue
    /// </summary>
    public class RuleError  // RECENTCHANGE - changed to class from struct
        //: IRuleError 
    {
        public RuleError(string message = null)
        {
            this.Message = message;
            category = null;
            suggestedFix = null;
            remarks = null;
        }

        public static implicit operator RuleError(string message)
        {
            return new RuleError(message);
        }
        
        #region Category

        /// <summary>
        /// A short one or two word description of the problem or problem area
        /// </summary>
        public string Category
        {
            get { return category; }
            set { category = value; }
        } private string category;

        #endregion
        
        #region Message

        /// <summary>
        /// A concise description of the error and how to fix it.
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        } private string message;

        #endregion


        #region Remarks

        /// <summary>
        /// Detailed secondary info about the error.
        /// </summary>
        public string Remarks
        {
            get { return remarks; }
            set { remarks = value; }
        } private string remarks;

        #endregion


        #region SuggestedFix

        public string SuggestedFix
        {
            get { return suggestedFix; }
            set { suggestedFix = value; }
        } private string suggestedFix;

        #endregion

    }
}
