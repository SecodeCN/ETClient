using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ETModel
{
	public sealed class Hotfix : Object
	{
		private Assembly assembly;

        private IStaticMethod start;

		public Action Update;
		public Action LateUpdate;
		public Action OnApplicationQuit;

		public Hotfix()
		{

		}

		public void GotoHotfix()
		{
			this.start?.Run();
		}

		public List<Type> GetHotfixTypes()
		{
			if (this.assembly == null)
			{
				return new List<Type>();
			}
			return this.assembly.GetTypes().ToList();
		}
	}
}