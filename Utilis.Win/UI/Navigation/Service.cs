﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilis.UI.Navigation.Win
{
    public class Service : IService
    {
        private readonly System.Windows.Controls.Frame m_frame;
        private readonly IViewMapper m_viewMapper;

        public Service ( System.Windows.Controls.Frame frame, IViewMapper viewMapper )
        {
            m_frame = frame;
            m_frame.Navigated += m_frame_Navigated;
            m_viewMapper = viewMapper;
        }

        void m_frame_Navigated ( object sender, System.Windows.Navigation.NavigationEventArgs e )
        {
            DoNavigated ( );
        }

        public bool CanGoBack ( )
        {
            return m_frame.CanGoBack;
        }

        public void GoBack ( )
        {
            m_frame.GoBack ( );
        }

        public void GoForward ( )
        {
            m_frame.GoForward ( );
        }

        public async Task<bool> Navigate<T_VM> ( T_VM parameter = default(T_VM) ) where T_VM : ViewModel.Base
        {
            var viewType = m_viewMapper.GetView<T_VM> ( );
            if ( viewType == null )
                throw new Exception ( "Unable to find view for ViewModel type '" + typeof ( T_VM ).Name + "'." );

            bool navResult = false;

            await Runner.RunOnDispatcherThread ( ( ) =>
            {
                var viewObject = Activator.CreateInstance ( viewType );
                var view = Contract.AssertIsType<IView> ( ( ) => viewObject, viewObject );

                view.ViewModelObject = parameter;
                CurrentViewModel = parameter;

                navResult = m_frame.Navigate ( view );
            } );
            return navResult;
        }

        public event Action Navigated;

        private void DoNavigated ( )
        {
            var navigated = Navigated;
            if ( navigated != null )
                navigated ( );
        }
        public ViewModel.Base CurrentViewModel { get; private set; }
    }
}
