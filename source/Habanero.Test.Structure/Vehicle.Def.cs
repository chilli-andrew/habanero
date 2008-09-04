//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// ------------------------------------------------------------------------------
// This partial class was auto-generated for use with the Habanero Architecture.
// NB Custom code should be placed in the provided stub class.
// Please do not modify this class directly!
// ------------------------------------------------------------------------------

namespace Habanero.Test.Structure
{
    using System;
    using Habanero.BO;
    using Habanero.Test.Structure;
    
    
    public partial class Vehicle : Entity
    {
        
        #region Properties
        public virtual Guid? VehicleID
        {
            get
            {
                return ((Guid?)(base.GetPropertyValue("VehicleID")));
            }
            set
            {
                base.SetPropertyValue("VehicleID", value);
            }
        }
        
        public virtual String VehicleType
        {
            get
            {
                return ((String)(base.GetPropertyValue("VehicleType")));
            }
            set
            {
                base.SetPropertyValue("VehicleType", value);
            }
        }
        
        public virtual DateTime? DateAssembled
        {
            get
            {
                return ((DateTime?)(base.GetPropertyValue("DateAssembled")));
            }
            set
            {
                base.SetPropertyValue("DateAssembled", value);
            }
        }
        
        public virtual Guid? OwnerID
        {
            get
            {
                return ((Guid?)(base.GetPropertyValue("OwnerID")));
            }
            set
            {
                base.SetPropertyValue("OwnerID", value);
            }
        }
        #endregion
        
        #region Relationships
        public virtual LegalEntity Owner
        {
            get
            {
                return Relationships.GetRelatedObject<LegalEntity>("Owner");
            }
            set
            {
                Relationships.SetRelatedObject("Owner", value);
            }
        }
        #endregion
    }
}