using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FourthSky {
	namespace Android {
		
		public class AIDLMethod {
			
			private static readonly Regex methodRegex = new Regex(@"(boolean|byte|char|short|int|long|float|double|String|[A-Za-z]+) ([A-Za-z]+)\(([A-Za-z0-9\s\,]*)\)(?:\sthrows\s)?([A-Za-z]*)?;");
			
			public AIDLInterface m_Interface;
			
			public string name;
			public Type returnType;
			public string returnTypeName;
			public List<AIDLParam> parameters;
			
			public AIDLMethod (AIDLInterface iface) {
				this.m_Interface = iface;
				
				parameters = new List<AIDLParam>();
			}
			
			public void ParseMethod(string method) {
				Match matcher = methodRegex.Match(method);
				if (matcher.Success) {
					// Group 1 is return type
					returnTypeName = matcher.Groups[1].Value;
					returnType = ParseType(returnTypeName);
					
					// Group 2 is method name
					name = matcher.Groups[2].Value;
					
					// Group 3 is method arguments
					Console.WriteLine("Method " + name + " params (" + matcher.Groups[3].Value.Split(',').Length + " length) " + matcher.Groups[3].Value + " - " + matcher.Groups[3].Value.Split(','));
					string[] methodParams = matcher.Groups[3].Value.Split(',');
					foreach(string parStr in methodParams) {
						if (parStr.Trim().Length > 0) {
							AIDLParam par = new AIDLParam(this);
							par.ParseParam(parStr.Trim());
							
							parameters.Add(par);
						}
					}
					
					// Group 4 is exception, handle later
					
				}
				
			}
			
			private Type ParseType(string paramType) {	
				// Parse C# type
				// Parse java.lang.String to C# string
				if (paramType == "String" ||
					paramType == "CharSequence") {
					return typeof(string);
					
				} else if (paramType == "void") {
					return typeof(void);
					
				} else if (paramType == "boolean") {
					return typeof(bool);
					
				} else if (paramType == "byte") {
					return typeof(byte);
					
				} else if (paramType == "char") {
					return typeof(char);
					
				} else if (paramType == "short") {
					return typeof(short);
					
				} else if (paramType == "int") {
					return typeof(int);
					
				} else if (paramType == "long") {
					return typeof(long);
					
				} else if (paramType == "float") {
					return typeof(float);
					
				} else if (paramType == "double") {
					return typeof(double);
					
				} else {
					// find class name from aidl interface
					m_Interface.importDict.TryGetValue(paramType, out returnTypeName);
					return typeof(AndroidJavaObject);
					
				}
			}
			
		}
		
	}
}

