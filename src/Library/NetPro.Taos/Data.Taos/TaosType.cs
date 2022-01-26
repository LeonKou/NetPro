// Copyright (c)  maikebing All rights reserved.
//// Licensed under the MIT License, See License.txt in the project root for license information.


namespace Maikebing.Data.Taos
{
    /// <summary>
    ///     Represents the type affinities used by columns in Taos tables.
    /// </summary>
  
    public enum TaosType
    {
        /// <summary>
        ///     A signed integer.
        /// </summary>
        Integer , 

        /// <summary>
        ///     A floating point value.
        /// </summary>
        Real  ,

        /// <summary>
        ///     A text string.
        /// </summary>
        Text ,

        /// <summary>
        ///     A blob of data.
        /// </summary>
        Blob 
    }
}
