using System;
using UnityEngine;

namespace FourthSky {
	namespace Android {
		
		public class AIDLParam {
				
			public enum Direction {
				IN, OUT, INOUT
			}
			
			/*
			private static readonly Type intType = typeof(int),
										 longType = typeof(long),
										 floatType = typeof(float),
										 doubleType = typeof(double),
										 boolType = typeof(bool),
										 stringType = typeof(string);
			*/
			
			public AIDLMethod method;
			
			public string name;
			public Direction direction;
			public string typeName;
			public Type type;
			
			// The class constructor is called when the class instance is created
			public AIDLParam (AIDLMethod method) {
				this.method = method;
			}
			
			public void ParseParam(string param) {
				string[] words = param.Split(' ');
				if (words.Length == 3) {
					direction = ParseDirection(words[0]);
					ParseType(words[1], out typeName, out type);
					name = words[2];
						
				} else if (words.Length == 2) {
					direction = Direction.IN;
					ParseType(words[0], out typeName, out type);
					name = words[1];
					
				} 
			}
			
			private Direction ParseDirection(string dir) {
				if (dir == "in") {
					return Direction.IN;
				} else if (dir == "out") {
					return Direction.OUT;
				} else if (dir == "inout") {
					return Direction.INOUT;
				}
				
				return Direction.IN;
			}
			
			private void ParseType(string paramType, out string tName, out Type t) {
				tName = paramType;
				
				// Parse C# type
				// Parse java.lang.String to C# string
				if (paramType == "String" || paramType == "java.lang.String" ||
					paramType == "CharSequence") {
					tName = "string";
					t = typeof(string);
					
				} else if (paramType == "void") {
					tName = paramType;
					t = typeof(void);
					
				} else if (paramType == "boolean") {
					tName = paramType;
					t = typeof(bool);
					
				} else if (paramType == "byte") {
					tName = paramType;
					t = typeof(byte);
					
				} else if (paramType == "char") {
					tName = paramType;
					t = typeof(char);
					
				} else if (paramType == "short") {
					tName = paramType;
					t = typeof(short);
					
				} else if (paramType == "int") {
					tName = paramType;
					t = typeof(int);
					
				} else if (paramType == "long") {
					tName = paramType;
					t = typeof(long);
					
				} else if (paramType == "float") {
					tName = paramType;
					t = typeof(float);
					
				} else if (paramType == "double") {
					tName = paramType;
					t = typeof(double);
					
				} else {
					string typeName = tName;
					method.m_Interface.importDict.TryGetValue(typeName, out tName);
					
					t = typeof(AndroidJavaObject);
					
				}
			}
			
			public Type ParamType {
				get {
					return type;
				}
			}
			
			public bool IsPrimitive {
				get {
					return type.IsPrimitive;
				}
			}
			
			public override string ToString() {
				return "AIDL parameter " + name + " (" + typeName + ")";
			}
		}
		
	}
}

