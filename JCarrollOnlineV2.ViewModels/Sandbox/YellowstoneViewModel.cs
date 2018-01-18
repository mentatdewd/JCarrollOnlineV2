using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Sandbox
{
    public class YellowstoneViewModel : ViewModelBase
    {
        private List<ImageFileMetaData> _imageFiles = new List<ImageFileMetaData>();

        public IEnumerable<ImageFileMetaData> ImageFiles
        {
            get
            {
                return _imageFiles;
            }
        }

        public void AddImageFile(ImageFileMetaData imageFileMetaData)
        {
            _imageFiles.Add(imageFileMetaData);
        }
    }
}
