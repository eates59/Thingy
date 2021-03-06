﻿using AppLib.Common.Console;
using System;
using System.Collections.Generic;

namespace Thingy.Infrastructure
{
    public class CommandLineParser
    {
        private IApplication _app;
        private Dictionary<string, Action> _switchActions;

        public CommandLineParser(IApplication app)
        {
            _app = app;
            _switchActions = new Dictionary<string, Action>();
            InitActions();
        }

        private void InitActions()
        {
            _switchActions.Add("/exit", () =>
            {
                _app.Close();
            });
            _switchActions.Add("/restart", () =>
            {
                _app.Restart();
            });
        }

        public void Parse(string args)
        {
            var parser = new ParameterParser(args, true, true);

            foreach (var action in _switchActions)
            {
                if (parser.StandaloneSwitches.Contains(action.Key))
                {
                    action.Value.Invoke();
                }
            }

            _app.HandleFiles(parser.Files);
        }
    }
}
