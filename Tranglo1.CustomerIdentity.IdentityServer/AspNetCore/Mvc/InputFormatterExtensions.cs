using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
	public static class InputFormatterExtensions
	{
		public static void RemoveMediaType(this InputFormatter inputFormatter, string mediaType)
		{
			if (inputFormatter != null)
			{
				if (inputFormatter.SupportedMediaTypes.Contains(mediaType))
				{
					inputFormatter.SupportedMediaTypes.Remove(mediaType);
				}
			}
		}
	}
}
