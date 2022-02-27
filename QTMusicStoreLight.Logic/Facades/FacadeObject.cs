﻿//@CodeCopy
//MdStart

using QTMusicStoreLight.Logic.Controllers;

namespace QTMusicStoreLight.Logic.Facades
{
    public abstract class FacadeObject
    {
        internal ControllerObject ControllerObject { get; private set; }

        protected FacadeObject(ControllerObject controllerObject)
        {
            ControllerObject = controllerObject;
        }
    }
}

//MdEnd
