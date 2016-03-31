﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace CommunityCoreLibrary
{

    public class MHD_PostLoadInjectors : IInjector
    {

        // TODO:  Alpha 13 API change
        // Obsoleted
        private static Dictionary<ModHelperDef,bool>    dictInjected;

        static                              MHD_PostLoadInjectors()
        {
            dictInjected = new Dictionary<ModHelperDef,bool>();
        }

#if DEBUG
        public string                       InjectString
        {
            get
            {
                return "Post Loaders injected";
            }
        }

        public bool                         IsValid( ModHelperDef def, ref string errors )
        {
            if( def.PostLoadInjectors.NullOrEmpty() )
            {
                return true;
            }

            bool isValid = true;

            foreach( var injectorType in def.PostLoadInjectors )
            {
                if(
                    ( injectorType == null )||
                    ( !injectorType.IsSubclassOf( typeof( SpecialInjector ) ) )
                )
                {
                    errors += string.Format( "Unable to resolve Post Load Injector '{0}'", injectorType.ToString() );
                    isValid = false;
                }
            }

            return isValid;
        }
#endif

        public bool                         Injected( ModHelperDef def )
        {
            if( def.PostLoadInjectors.NullOrEmpty() )
            {
                return true;
            }

            bool injected;
            if( !dictInjected.TryGetValue( def, out injected ) )
            {
                return false;
            }

            return injected;
        }

        public bool                         Inject( ModHelperDef def )
        {
            if( def.PostLoadInjectors.NullOrEmpty() )
            {
                return true;
            }

            foreach( var injectorType in def.PostLoadInjectors )
            {
                var injectorObject = (SpecialInjector) Activator.CreateInstance( injectorType );
                if( injectorObject == null )
                {
                    CCL_Log.Message( string.Format( "Unable to create instance of '{0}'", injectorType.ToString() ) );
                    return false;
                }
                // TODO: Alpha 13 API change
                /*
                if( !injectorObject.Inject() )
                {
                    CCL_Log.Message( string.Format( "Error injecting '{0}'", injectorType.ToString() ) );
                    return false;
                }
                */
                try
                {
                    injectorObject.Inject();
                }
                catch
                {
                    CCL_Log.Message( string.Format( "Error injecting '{0}'", injectorType.ToString() ) );
                    return false;
                }
            }

            dictInjected.Add( def, true );
            return true;
        }

    }

}
