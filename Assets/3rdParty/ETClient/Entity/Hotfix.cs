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
#if ILRuntime
		private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
#else
		private Assembly assembly;
#endif

		private IStaticMethod start;

		public Action Update;
		public Action LateUpdate;
		public Action OnApplicationQuit;

		public Hotfix()
		{

		}

		public void GotoHotfix()
		{
#if ILRuntime
			ILHelper.InitILRuntime(this.appDomain);
#endif
			this.start.Run();
		}

		public List<Type> GetHotfixTypes()
		{
#if ILRuntime
			if (this.appDomain == null)
			{
				return new Type[0];
			}

			return this.appDomain.LoadedTypes.Values.Select(x => x.ReflectionType);
#else
			if (this.assembly == null)
			{
				return new List<Type>();
			}
			return this.assembly.GetTypes().ToList();
#endif
		}
	}
}