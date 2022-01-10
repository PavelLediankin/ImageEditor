using System;
using System.Drawing;
using System.Windows.Forms;
using MyPhotoshop.Data;
using MyPhotoshop.Filters;
using MyPhotoshop.Parameters;
using MyPhotoshop.Window;
using Ninject;

namespace MyPhotoshop
{
	class MainClass
	{
        [STAThread]
		public static void Main (string[] args)
		{
            var container = RegisterDependensies();

			Application.Run (container.Get<MainWindow>());
		}

        private static StandardKernel RegisterDependensies()
        {
            var container = new StandardKernel();

            RegisterBase(container);
            FilterRegistrator.Register(container);
            RegisterUi(container);

            return container;
        }

        private static void RegisterBase(StandardKernel container)
        {
            container.Bind<IProcessor>().To<Processor>();
            container.Bind<IPhotoStackSaver>().To<PhotoStackSaver>();
        }

        private static void RegisterUi(StandardKernel container)
        {

        }
    }
}
