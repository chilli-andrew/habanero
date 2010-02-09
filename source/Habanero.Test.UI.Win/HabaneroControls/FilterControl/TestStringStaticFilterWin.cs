using Habanero.Test.UI.Base.FilterController;
using Habanero.UI.Base;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Win.HabaneroControls
{
    [TestFixture]
    public class TestStringStaticFilterWin : TestStringStaticFilter
    {
        protected override IControlFactory GetControlFactory() { return new ControlFactoryWin(); }
    }
}