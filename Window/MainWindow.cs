using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MyPhotoshop.Data;
using MyPhotoshop.Filters;

namespace MyPhotoshop.Window
{
    public class MainWindow : Form
    {
        private readonly IProcessor processor;
        private readonly IPhotoStackSaver stackSaver;
        Bitmap originalBmp;
        Photo originalPhoto;
        PictureBox original;
        PictureBox processed;
        List<ComboBox> filtersGroups;
        Panel parametersPanel;
        List<NumericUpDown> parametersControls;
        List<Button> apply;
        Button choose;
        Button save;
        Button undo;
        Button redo;
        Button initialPhoto;

        public MainWindow(IProcessor processor, IFilter[] filters, IPhotoStackSaver stackSaver)
        {
            this.processor = processor;
            this.stackSaver = stackSaver;
            var groups = FilterGroupper.GetGrouppedFilters(filters).ToList();

            original = new PictureBox();
            Controls.Add(original);

            processed = new PictureBox();
            Controls.Add(processed);

            filtersGroups = new List<ComboBox>();
            apply = new List<Button>();

            InitFilterGroups(groups.Count);

            AddChoosePhotoButton();

            AddSavePhotoButton();

            AddUndoButton();

            AddRedoButton();

            AddOriginalButton();

            AddFilter(groups);

            Text = "Mini-Photoshop";
            FormBorderStyle = FormBorderStyle.FixedDialog;

            var photo = (Bitmap)Image.FromFile("StartPhoto.jpg");
            InitBitmap(photo, new Size(photo.Width, photo.Height));
        }

        private void AddOriginalButton()
        {
            initialPhoto = new Button();
            initialPhoto.Text = "Вернуть исходное фото";
            initialPhoto.Enabled = false;
            initialPhoto.Width = 150;
            initialPhoto.Left = 400;
            initialPhoto.Click += BackToOriginal;
            Controls.Add(initialPhoto);
        }

        private void AddUndoButton()
        {
            undo = new Button();
            undo.Text = "Отменить";
            undo.Enabled = false;
            undo.Width = 100;
            undo.Left = 200;
            undo.Click += UndoPhoto;
            Controls.Add(undo);
        }

        private void AddRedoButton()
        {
            redo = new Button();
            redo.Text = "Повторить";
            redo.Enabled = false;
            redo.Width = 100;
            redo.Left = 300;
            redo.Click += RedoPhoto;
            Controls.Add(redo);
        }

        private void AddSavePhotoButton()
        {
            save = new Button();
            save.Text = "Сохранить фото";
            save.Enabled = false;
            save.Width = 100;
            save.Click += SavePhoto;
            Controls.Add(save);
        }

        private void AddChoosePhotoButton()
        {
            choose = new Button();
            choose.Text = "Выбрать фото";
            choose.Width = 100;
            choose.Left = 100;
            choose.Click += ChoosePhoto;
            Controls.Add(choose);
        }

        private void InitFilterGroups(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var box = new ComboBox();
                box.DropDownStyle = ComboBoxStyle.DropDownList;
                var index = i;
                box.SelectedIndexChanged += (sender1, e1) => FilterChanged(sender1, e1, index);
                box.Enabled = false;
                Controls.Add(box);
                filtersGroups.Add(box);

                var button = new Button();
                button.Text = "Применить";
                button.Click += (sender1, e1) => Process(sender1, e1, index);
                apply.Add(button);
                Controls.Add(button);
            }
        }

        public void InitBitmap(Bitmap bmp, Size size)
        {
            originalBmp = bmp;
            originalPhoto = Convertors.Bitmap2Photo(bmp);

            original.Image = originalBmp;
            original.Left = 0;
            original.Top = 30;
            original.Width = size.Width;
            original.Height = size.Height;
            original.ClientSize = originalBmp.Size;

            for (var i = 0; i < filtersGroups.Count; i++)
            {
                filtersGroups[i].Left = original.Right + 10;
                filtersGroups[i].Top = 30 + i * 100;
                filtersGroups[i].Width = 200;
                filtersGroups[i].Height = 20;
                apply[i].Enabled = false;
                apply[i].Left = filtersGroups[i].Left + 50;
                apply[i].Top = 85 + i * 100;
                apply[i].Width = 100;
                apply[i].Height = 40;
            }

            ClientSize = new Size(filtersGroups[0].Right + 20, original.Bottom);
        }

        public void LoadBitmap(Bitmap bmp, Size size)
        {
            originalBmp = bmp;
            originalPhoto = Convertors.Bitmap2Photo(bmp);

            original.Image = originalBmp;
            original.Left = 0;
            original.Top = 30;
            original.Width = size.Width;
            original.Height = size.Height;
            original.ClientSize = originalBmp.Size;

            processed.Left = 0;
            processed.Top = 30;
            processed.Size = original.Size;

            ClientSize = new Size(filtersGroups[0].Right + 20, processed.Bottom);
        }

        public void AddFilter(List<FiltersGroup> filters)
        {
            for (var i = 0; i < filters.Count; i++)
            {
                foreach (var filter in filters[i].Filters)
                {
                    filtersGroups[i].Items.Add(filter);
                    if (filtersGroups[i].SelectedIndex != -1)
                        continue;
                    apply[i].Enabled = true;
                }
            }
        }

        void BackToOriginal(object sender, EventArgs e)
        {
            processed.Image = Convertors.Photo2Bitmap(originalPhoto);
            stackSaver.Clear();
            AddPhoto(Convertors.Bitmap2Photo(new Bitmap(processed.Image)));
        }

        void FilterChanged(object sender, EventArgs e, int i)
        {
            apply[i].Enabled = true;
            for (var j = 0; j < filtersGroups.Count; j++)
                if (j != i)
                    apply[j].Enabled = false;
            var filter = (IFilter)filtersGroups[i].SelectedItem;
            if (filter == null) return;
            if (parametersPanel != null) Controls.Remove(parametersPanel);
            parametersControls = new List<NumericUpDown>();
            parametersPanel = new Panel();
            parametersPanel.Left = filtersGroups[i].Left;
            parametersPanel.Top = filtersGroups[i].Bottom + 10;
            parametersPanel.Width = filtersGroups[i].Width;
            parametersPanel.Height = ClientSize.Height - parametersPanel.Top;

            int y = 0;

            foreach (var param in filter.GetParameters())
            {
                var label = new Label();
                label.Left = 0;
                label.Top = y;
                label.Width = parametersPanel.Width - 50;
                label.Height = 20;
                label.Text = param.Name;
                parametersPanel.Controls.Add(label);

                var box = new NumericUpDown();
                box.Left = label.Right;
                box.Top = y;
                box.Width = 50;
                box.Height = 20;
                box.Value = (decimal)param.DefaultValue;
                box.Increment = (decimal)param.Increment / 3;
                box.Maximum = (decimal)param.MaxValue;
                box.Minimum = (decimal)param.MinValue;
                box.DecimalPlaces = 2;
                parametersPanel.Controls.Add(box);
                y += label.Height + 5;
                parametersControls.Add(box);
            }
            Controls.Add(parametersPanel);
        }

        void Process(object sender, EventArgs empty, int i)
        {
            original.Visible = false;
            var parameters = parametersControls.Select(z => (double)z.Value).ToArray();
            var filter = (IFilter)filtersGroups[i].SelectedItem;
            var previousPhoto = stackSaver.CurrentPhoto ?? originalPhoto;
            var result = processor.ProcessPhoto(filter, previousPhoto, parameters);
            var resultBmp = Convertors.Photo2Bitmap(result);
            DrawProcessedImage(resultBmp);
            if (!stackSaver.CanUndo)
                AddPhoto(Convertors.Bitmap2Photo(new Bitmap(original.Image)));
            AddPhoto(Convertors.Bitmap2Photo(new Bitmap(processed.Image)));
        }

        void DrawProcessedImage(Bitmap result)
        {
            if (result.Width > originalBmp.Width || result.Height > originalBmp.Height)
            {
                float k = Math.Min((float)originalBmp.Width / result.Width, (float)originalBmp.Height / result.Height);
                var newBmp = new Bitmap((int)(result.Width * k), (int)(result.Height * k));
                using (var g = Graphics.FromImage(newBmp))
                {
                    g.DrawImage(result, new Rectangle(0, 0, newBmp.Width, newBmp.Height), new Rectangle(0, 0, result.Width, result.Height), GraphicsUnit.Pixel);
                }
                result = newBmp;
            }
            initialPhoto.Enabled = true;
            undo.Enabled = true;
            save.Enabled = true;
            processed.Image = result;
        }

        void ChoosePhoto(object sender, EventArgs empty)
        {
            Bitmap image;

            OpenFileDialog open_dialog = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"
            };
            if (open_dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image = new Bitmap(open_dialog.FileName);
                    var newWidth = image.Width;
                    var newHeight = image.Height;
                    if (newWidth > 480 || newHeight > 300)
                    {
                        var t = 1.0;
                        if (image.Width > image.Height)
                        {
                            t = 480.0 / image.Width;
                        }
                        else
                        {
                            t = 300.0 / image.Height;
                        }
                        newWidth = (int)(newWidth * t);
                        newHeight = (int)(newHeight * t);
                    }
                    var newSize = new Size(newWidth, newHeight);
                    var newImage = new Bitmap(image, newSize);
                    processed.Image = null;
                    save.Enabled = false;
                    for (var i = 0; i < 3; i++)
                        filtersGroups[i].Enabled = true;
                    LoadBitmap(newImage, newSize);
                    original.Invalidate();
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void SavePhoto(object sender, EventArgs empty)
        {
            if (processed.Image == null)
                return;
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить картинку как...";
            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.Filter = "Image Files(Image Files(*.JPG)|*.JPG|Image Files(*.PNG)|*.PNG|*.BMP)|*.BMP|Image Files(*.GIF)|*.GIF|All files (*.*)|*.*";
            savedialog.ShowHelp = true;
            if (savedialog.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                processed.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch
            {
                MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void UndoPhoto(object sender, EventArgs empty)
        {
            if (stackSaver.CanUndo)
            {
                var photo = stackSaver.Undo();
                processed.Image = Convertors.Photo2Bitmap(photo);
                redo.Enabled = true;
                if (!stackSaver.CanUndo)
                    undo.Enabled = false;
            }
        }

        void RedoPhoto(object sender, EventArgs empty)
        {
            if (stackSaver.CanRedo)
            {
                var photo = stackSaver.Redo();
                processed.Image = Convertors.Photo2Bitmap(photo);
                undo.Enabled = true;
                if (!stackSaver.CanRedo)
                    redo.Enabled = false;
            }
        }

        void AddPhoto(Photo photo)
        {
            stackSaver.Do(photo);
            redo.Enabled = false;
        }
    }
}
