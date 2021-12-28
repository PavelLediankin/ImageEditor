using System;
using System.Collections.Generic;

namespace MyPhotoshop.Data
{
    public class PhotoStackSaver : IPhotoStackSaver
    {
        public bool CanUndo => addedPhotos.Count > 0;
        public bool CanRedo => removedPhotos.Count > 0;
        public Photo CurrentPhoto { get; private set; }

        private readonly Stack<Photo> addedPhotos = new Stack<Photo>();
        private readonly Stack<Photo> removedPhotos = new Stack<Photo>();

        public Photo Undo()
        {
            SwitchPhotos(addedPhotos, removedPhotos);
            return CurrentPhoto;
        }

        public Photo Redo()
        {
            SwitchPhotos(removedPhotos, addedPhotos);
            return CurrentPhoto;
        }

        private void SwitchPhotos(Stack<Photo> first, Stack<Photo> second)
        {
            second.Push(CurrentPhoto);
            CurrentPhoto = first.Pop();
        }

        public void Do(Photo photo)
        {
            if (photo == null)
                throw new ArgumentNullException("", "You tried to add null Photo to stack");
            if(CurrentPhoto != null)
                addedPhotos.Push(CurrentPhoto);
            CurrentPhoto = photo;
            
            removedPhotos.Clear();
        }

        public void Clear()
        {
            addedPhotos.Clear();
            removedPhotos.Clear();
            CurrentPhoto = null;
        }
    }
}