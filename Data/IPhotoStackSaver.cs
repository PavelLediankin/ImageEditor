namespace MyPhotoshop.Data
{
    public interface IPhotoStackSaver
    {
        /// <summary>
        /// Используется для проверки можно ли отменить фото
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Используется для проверки можно ли вернуть отмененное фото
        /// </summary>
        bool CanRedo { get; }

        Photo CurrentPhoto { get; }
        /// <summary>
        /// Возвращает последнее фото из стэка(отменяет фото), если в стэке ничего нет бросает исключение
        /// </summary>
        /// <returns></returns>
        Photo Undo();

        /// <summary>
        /// Возвращает последнее фото из стэка отменненных фото, если в стэке ничего нет бросает исключение
        /// </summary>
        /// <returns></returns>
        Photo Redo();

        /// <summary>
        /// Складывает фото в стэк, очищает стэк отменненных фото
        /// </summary>
        /// <returns></returns>
        void Do(Photo photo);

        void Clear();
    }
}