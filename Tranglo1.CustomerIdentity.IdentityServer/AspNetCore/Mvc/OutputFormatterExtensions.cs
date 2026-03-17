using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
	public static class OutputFormatterExtensions
	{
		public static void RemoveMediaType(this OutputFormatter outputFormatter, string mediaType)
		{
			if (outputFormatter != null)
			{
				if (outputFormatter.SupportedMediaTypes.Contains(mediaType))
				{
					outputFormatter.SupportedMediaTypes.Remove(mediaType);
				}
			}
		}
	}
}
