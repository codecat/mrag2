using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mrag2Demo
{
	class Program
	{
		static void Main(string[] args)
		{
			using(var game = new DemoGame()) {
				game.Run();
			}
		}
	}
}
