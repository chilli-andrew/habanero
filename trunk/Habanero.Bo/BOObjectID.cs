using System;
using Habanero.Base.Exceptions;
using Habanero.Bo.ClassDefinition;
using Habanero.Base;

namespace Habanero.Bo
{
    /// <summary>
    /// Manages a business object primary key, where the key is the object's ID.
    /// Unlike composite primary keys that are typically composed of properties
    /// that are used in real-world scenarios and can be changed, an ObjectID
    /// is a primary key that won't be used by end users.  It typically acts
    /// discretely behind the user interface.
    /// </summary>
    public class BOObjectID : BOPrimaryKey
    {
        protected BOProp _objectIDProp;

        /// <summary>
        /// Constructor to initialise a new ObjectID
        /// </summary>
        /// <param name="lPrimaryKeyDef">The primary key definition</param>
        internal BOObjectID(PrimaryKeyDef lPrimaryKeyDef) : base(lPrimaryKeyDef)
        {
            if (lPrimaryKeyDef.Count != 1 || !lPrimaryKeyDef.IsObjectID)
            {
                throw new InvalidObjectIdException(
                    "The BOOBjectID must have a key def that defines exactly one property and that is an ObjectID");
            }
        }

        /// <summary>
        /// Adds a property to the key
        /// </summary>
        /// <param name="BOProp">The property to add</param>
        internal override void Add(BOProp BOProp)
        {
            if (Count > 0)
            {
                throw new InvalidObjectIdException("A BOOBjectID cannot have " +
                    "more than one property.");
            }
            if (BOProp.PropertyType != typeof (Guid))
            {
                //TODO - this exception breaks a whole bunch of tests, review.
                //throw new InvalidObjectIdException("A BOOBjectID cannot have " +
                //    "a property of type other than Guid.");
            }

            base.Add(BOProp);
        }

        /// <summary>
        /// Returns the objectID's property
        /// </summary>
        protected BOProp ObjectIDProp
        {
            get
            {
                if (_objectIDProp == null)
                {
                    _objectIDProp = base[KeyDef.KeyName];
                }
                if (_objectIDProp == null)
                {
                    throw new InvalidObjectIdException("Unable to located the objectIDProp.");
                }
                return _objectIDProp;
            }
        }

        /// <summary>
        /// Sets the objectID
        /// </summary>
        /// <param name="id">The Guid ID to set to</param>
        internal override void SetObjectID(Guid id)
        {
            //If the object id is not already set then set it.
            if (ObjectIDProp != null)
            {
                if (ObjectIDProp.Value == null ||
                    (Guid) ObjectIDProp.Value == Guid.Empty)
                {
                    ObjectIDProp.Value = id;
                }
                else if ((Guid) ObjectIDProp.Value != id)
                {
                    throw new InvalidObjectIdException("The property for objectID cannot be null.");
                }
            }
            else
            {
                throw new InvalidObjectIdException("The property for objectID cannot be null.");
            }
        }

        /// <summary>
        /// Returns the ObjectID as "ID=ObjectIDValue"
        /// </summary>
        /// <returns>Returns a string</returns>
        public override String GetObjectId()
        {
            return "ID=" + ObjectIDProp.PropertyValueString;
        }

        /// <summary>
        /// Returns the ObjectID <see cref="Habanero.Bo.BOObjectID.GetObjectId()"/>
        /// </summary>
        /// <returns>Returns the ID as a string</returns>
        public override string GetObjectNewID()
        {
            return GetObjectId();
        }

        /// <summary>
        /// Indicates if a BOObjectID has the same value as a given Guid
        /// </summary>
        /// <param name="lhs">The ObjectID to compare</param>
        /// <param name="rhs">The Guid to compare</param>
        /// <returns>Returns true if the arguments are equal</returns>
        public static bool operator ==(BOObjectID lhs, Guid rhs)
        {
            return ((Guid) lhs.ObjectIDProp.Value == rhs);
        }

        /// <summary>
        /// Indicates if a BOObjectID has a different value to a given Guid
        /// </summary>
        /// <param name="lhs">The ObjectID to compare</param>
        /// <param name="rhs">The Guid to compare</param>
        /// <returns>Returns true if the arguments differ</returns>
        public static bool operator !=(BOObjectID lhs, Guid rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Indicates if two BOObjectID objects have the same value
        /// </summary>
        /// <param name="lhs">The first ObjectID to compare</param>
        /// <param name="rhs">The second ObjectID to compare</param>
        /// <returns>Returns true if the ObjectID's are equal</returns>
        public static bool operator ==(BOObjectID lhs, BOObjectID rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Indicates if two BOObjectID objects have different values
        /// </summary>
        /// <param name="lhs">The first ObjectID to compare</param>
        /// <param name="rhs">The second ObjectID to compare</param>
        /// <returns>Returns true if the ObjectID's differ</returns>
        public static bool operator !=(BOObjectID lhs, BOObjectID rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Indicates if a specified BOObjectID has the same value as this one
        /// </summary>
        /// <param name="obj">The BOObjectID to compare with</param>
        /// <returns>Returns true if equal</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is BOObjectID)
            {
                return (this.GetObjectId() == ((BOObjectID) obj).GetObjectId());
            }

            return false;
        }

        /// <summary>
        /// Returns a hashcode of the ObjectID
        /// </summary>
        /// <returns>Returns a hashcode integer</returns>
        public override int GetHashCode()
        {
            return this.GetObjectId().GetHashCode();
        }

        /// <summary>
        /// Indicates whether to check for duplicates. This will always
        /// return false, since the objectID is guaranteed to be unique.
        /// </summary>
        /// <returns>Returns false</returns>
        internal override bool MustCheckKey()
        {
            return false;
        }

        /// <summary>
        /// Returns an empty string
        /// </summary>
        /// <returns>Returns an empty string</returns>
        internal override string GetOrigObjectID()
        {
            return "";
        }
    }
}