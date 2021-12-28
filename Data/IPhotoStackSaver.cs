namespace MyPhotoshop.Data
{
    public interface IPhotoStackSaver
    {
        /// <summary>
        /// ������������ ��� �������� ����� �� �������� ����
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// ������������ ��� �������� ����� �� ������� ���������� ����
        /// </summary>
        bool CanRedo { get; }

        Photo CurrentPhoto { get; }
        /// <summary>
        /// ���������� ��������� ���� �� �����(�������� ����), ���� � ����� ������ ��� ������� ����������
        /// </summary>
        /// <returns></returns>
        Photo Undo();

        /// <summary>
        /// ���������� ��������� ���� �� ����� ����������� ����, ���� � ����� ������ ��� ������� ����������
        /// </summary>
        /// <returns></returns>
        Photo Redo();

        /// <summary>
        /// ���������� ���� � ����, ������� ���� ����������� ����
        /// </summary>
        /// <returns></returns>
        void Do(Photo photo);

        void Clear();
    }
}