﻿// Retrieved from http://stackoverflow.com/questions/380595/multimap-in-net
// Modifications:
//  - In-class implementation of ArgumentVerifier method
//  - Namespace
//  - This was a MultiValueDictionary class, but is now changed to be a MultiValueSortedList

//////////////////////////////////////////////////////////////////////
// Algorithmia is (c) 2008 Solutions Design. All rights reserved.
// http://www.sd.nl
//////////////////////////////////////////////////////////////////////
// COPYRIGHTS:
// Copyright (c) 2008 Solutions Design. All rights reserved.
// 
// The Algorithmia library sourcecode and its accompanying tools, tests and support code
// are released under the following license: (BSD2)
// ----------------------------------------------------------------------
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met: 
//
// 1) Redistributions of source code must retain the above copyright notice, this list of 
//    conditions and the following disclaimer. 
// 2) Redistributions in binary form must reproduce the above copyright notice, this list of 
//    conditions and the following disclaimer in the documentation and/or other materials 
//    provided with the distribution. 
// 
// THIS SOFTWARE IS PROVIDED BY SOLUTIONS DESIGN ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL SOLUTIONS DESIGN OR CONTRIBUTORS BE LIABLE FOR 
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
//
// The views and conclusions contained in the software and documentation are those of the authors 
// and should not be interpreted as representing official policies, either expressed or implied, 
// of Solutions Design. 
//
//////////////////////////////////////////////////////////////////////
// Contributers to the code:
//      - Frans  Bouma [FB]
//////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using SD.Tools.Algorithmia.UtilityClasses;

namespace LionFire.Collections
{
    /// <summary>
    /// Extension to the normal SortedList. This class can store more than one value for every key. It keeps a HashSet for every Key value.
    /// Calling Add with the same Key and multiple values will store each value under the same Key in the SortedList. Obtaining the values
    /// for a Key will return the HashSet with the Values of the Key. 
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class MultiValueSortedList<TKey, TValue> : SortedList<TKey, HashSet<TValue>>
    {
        private void ArgumentVerifierCantBeNull(object obj, string msg)
        {
            if (obj == null) throw new ArgumentNullException(msg);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueSortedList&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        public MultiValueSortedList()

        {
#if AOT
			if(typeof(TKey).IsValueType)
			throw new NotSupportedException("On AOT, must specify converter when key type is a ValueType");
#endif
        }
        public MultiValueSortedList(IComparer<TKey> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Adds the specified value under the specified key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            ArgumentVerifierCantBeNull(key, "key");

            HashSet<TValue> container = null;
            if (!this.TryGetValue(key, out container))
            {
                container = new HashSet<TValue>();
                base.Add(key, container);
            }
            container.Add(value);
        }


        /// <summary>
        /// Determines whether this SortedList contains the specified value for the specified key 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if the value is stored for the specified key in this SortedList, false otherwise</returns>
        public bool ContainsValue(TKey key, TValue value)
        {
            ArgumentVerifierCantBeNull(key, "key");
            bool toReturn = false;
            HashSet<TValue> values = null;
            if (this.TryGetValue(key, out values))
            {
                toReturn = values.Contains(value);
            }
            return toReturn;
        }


        /// <summary>
        /// Removes the specified value for the specified key. It will leave the key in the SortedList unless there are no more values for the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Remove(TKey key, TValue value)
        {
            ArgumentVerifierCantBeNull(key, "key");

            HashSet<TValue> container = null;
            if (this.TryGetValue(key, out container))
            {
                container.Remove(value);
                if (container.Count <= 0)
                {
                    this.Remove(key);
                }
            }
        }


        /// <summary>
        /// Merges the specified multivalueSortedList into this instance.
        /// </summary>
        /// <param name="toMergeWith">To merge with.</param>
        public void Merge(MultiValueSortedList<TKey, TValue> toMergeWith)
        {
            if (toMergeWith == null)
            {
                return;
            }

            foreach (KeyValuePair<TKey, HashSet<TValue>> pair in (IEnumerable)toMergeWith)
            {
                foreach (TValue value in pair.Value)
                {
                    this.Add(pair.Key, value);
                }
            }
        }


        /// <summary>
        /// AsyncGetsWithEvents the values for the key specified. This method is useful if you want to avoid an exception for key value retrieval and you can't use TryGetValue
        /// (e.g. in lambdas)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="returnEmptySet">if set to true and the key isn't found, an empty hashset is returned, otherwise, if the key isn't found, null is returned</param>
        /// <returns>
        /// This method will return null (or an empty set if returnEmptySet is true) if the key wasn't found, or
        /// the values if key was found.
        /// </returns>
        public HashSet<TValue> GetValues(TKey key, bool returnEmptySet)
        {
            HashSet<TValue> toReturn = null;
            if (!base.TryGetValue(key, out toReturn) && returnEmptySet)
            {
                toReturn = new HashSet<TValue>();
            }
            return toReturn;
        }
    }
}
