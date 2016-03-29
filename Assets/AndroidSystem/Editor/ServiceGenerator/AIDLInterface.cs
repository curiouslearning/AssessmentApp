using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FourthSky {
	namespace Android {
	
		public class AIDLInterface {
			
			private static readonly Regex pkgRegex = new Regex(@"package ([A-Za-z0-9\-\.]+);");
			private static readonly Regex importRegex = new Regex(@"import ([A-Za-z0-9\-\.]+);");
			private static readonly Regex interfaceRegex = new Regex(@"interface ([A-Za-z]+) {");			
			private static readonly Regex methodRegex = new Regex(@"(boolean|byte|char|short|int|long|float|double|String|[A-Za-z]+) ([A-Za-z0-9]+)\(([A-Za-z0-9\s\,]*)\)(?:\sthrows\s)?([A-Za-z]*)?;");
			
			public string name;
			public string packageName;
			public Dictionary<string, string> importDict;
			public List<AIDLMethod> methods;		
			
			public AIDLInterface () {
				methods = new List<AIDLMethod>();
				
				importDict = new Dictionary<string, string>();
			}
			
			public string ClassName {
				get {
					return packageName + "." + name;
				}
			}
			
			public void ParseInterface(string code) {
				using (StringReader reader = new StringReader(code)) {
					string line;
	    			while ((line = reader.ReadLine()) != null) {
						// Get package
						Match matcher = pkgRegex.Match(line);
						if (matcher.Success) {
							packageName = matcher.Groups[1].Value;
							
							continue;
						}
						
						// Get imports
						matcher = importRegex.Match(line);
						if (matcher.Success) {
							// Save imports in dictionary
							string fullName = matcher.Groups[1].Value;
							importDict.Add(fullName.Substring(fullName.LastIndexOf('.') + 1), fullName);
							
							continue;
						}
						
						// Get interface name
						matcher = interfaceRegex.Match(line);
						if (matcher.Success) {
							name = matcher.Groups[1].Value;
							
							continue;
						}
						
						// Parse method declaration
						matcher = methodRegex.Match(line);
						if (matcher.Success) {
							AIDLMethod method = new AIDLMethod(this);
							method.ParseMethod(line);
							methods.Add(method);
							
							continue;
						}						
						
					}
					
				}
			
			}
				
		}
		
	}
}

