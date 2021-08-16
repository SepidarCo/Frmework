namespace Sepidar.BlobManagement
{
    public interface IBlobRepository
    {
        string Add(Blob blob);

        Blob Get(string token);

        Blob GetByName(string name);

        void Remove(string token);
    }
}