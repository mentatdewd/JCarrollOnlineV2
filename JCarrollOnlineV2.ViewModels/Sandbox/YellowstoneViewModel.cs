using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Sandbox
{
    public class YellowstoneViewModel : ViewModelBase
    {
        private readonly List<ImageFileMetadata> _imageFiles = new List<ImageFileMetadata>();

        public IEnumerable<ImageFileMetadata> ImageFiles => _imageFiles;

        public void AddImageFile(ImageFileMetadata imageFileMetadata)
        {
            _imageFiles.Add(imageFileMetadata);
        }
    }
}
