namespace Sepidar.BlobManagement
{
    public abstract class BlobStorage
    {
        public abstract BlobStorageType StorageType { get;  }
    }
    public class DiskBlobStorage : BlobStorage
    {
        public override BlobStorageType StorageType
        {
            get
            {
                return BlobStorageType.Disk; 

            }
        }
    }
    public class DatabaseBlobStorage : BlobStorage
    {
        public override BlobStorageType StorageType
        {
            get
            {
                return BlobStorageType.Database;
            }
        }

    }
    public class GoogleDriveBlobStorage : BlobStorage
    {
        public override BlobStorageType StorageType
        {
            get
            {
                return BlobStorageType.GoogleDrive;

            }
        }
    }
}
