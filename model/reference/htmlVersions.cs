using System;
using System.Xml;

namespace Bakera.Hatomaru{

	[FlagsAttribute]
	public enum HtmlVersions{
		none = 0,
		h10 = 1,
		h20 = 1 << 1,
		h2x = 1 << 2,
		h30 = 1 << 3,
		hp = 1 << 4,
		h32 = 1 << 5,
		h40 = 1 << 6,
		h40t = 1 << 7,
		h40f = 1 << 8,
		h401 = 1 << 9,
		h401t = 1 << 10,
		h401f = 1 << 11,
		x10 = 1 << 12,
		x10t = 1 << 13,
		x10f = 1 << 14,
		x11 = 1 << 15,
		x20 = 1 << 16,
		h5 = 1 << 17
	}

}

