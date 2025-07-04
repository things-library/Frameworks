namespace Starlight.Entity.Tests.Integration.Base
{    
    public interface IBaseTests
    {
        public void StoreType();

        public void InsertUpdateDelete();

        public void InsertDelete_InheritedClass();

        public void InsertDeleteNullible();

        public void UpsertUpsertAndDelete();

        public void InsertTwice();

        public void GetEntities();
    }
}
