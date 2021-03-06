#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Habanero.Util;

namespace Habanero.Base.DataMappers
{
    ///<summary>
    /// data mapper that is used for  reading and writing byte[] (Byte array) from a database
    ///</summary>
    public class ByteArrayDataMapper : DataMapper
    {
        public override bool TryParsePropValue(object valueToParse, out object returnValue)
        {
            if (base.TryParsePropValue(valueToParse, out returnValue)) return true;
            if (valueToParse is byte[])
            {
                returnValue = valueToParse;
                return true;
            } 
            if (valueToParse is String)
            {
                var bytes = Convert.FromBase64String((string) valueToParse);
                returnValue = bytes;
                return true;
            }
            returnValue = null;
            return false;
        }

        public override string ConvertValueToString(object value)
        {
            if (value is byte[])
            {
                return Convert.ToBase64String((byte[]) value);
            }
            return Convert.ToString(value);
        }
    }
}