using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFBot.Dialogs
{

    public class SFBotDialogFactory : DialogFactory, ISFBotDialogFactory
    {
        public SFBotDialogFactory(IComponentContext scope) : base(scope)
        {
            
        }
    }
}