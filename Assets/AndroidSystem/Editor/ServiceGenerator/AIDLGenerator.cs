using System;
using UnityEngine;

namespace FourthSky {
	namespace Android {
		
		public class AIDLGenerator
		{
			public AIDLGenerator ()
			{
			}
			
			
			public static string GenerateBinder(AIDLInterface iface) {
				// Imports
				string code = "using UnityEngine;\n" +
							  "using System;\n" +
							  "using System.Collections;\n" +
							  "using FourthSky.Android;\n\n";
				
				// Class declaration
				code += "public class " + iface.name + " : AndroidServiceBinder {\n\n";
				
				// Ids for method transactions
				int idx = 0;
				foreach (AIDLMethod method in iface.methods) {
					code += "\tprivate static readonly int TRANSACTION_" + method.name + " = AndroidServiceBinder.FIRST_CALL_TRANSACTION + " + idx++ + ";\n";
				}
				code += "\n";
				
				// Constructor
				code += "\tprotected " + iface.name + "(AndroidJavaObject binder)\n" +
						"\t\t: base(\"" + iface.ClassName + "\", binder) {\n\n" +
						"\t}\n\n";
				
				// Wrap methods
				code += "\tpublic static " + iface.name + " Wrap(IntPtr binderPtr) {\n" +
						"\t\treturn new " + iface.name + "(AndroidSystem.ConstructJavaObjectFromPtr(binderPtr));\n\n" +
						"\t}\n\n";
				
				code += "\tpublic static " + iface.name + " Wrap(AndroidJavaObject binder) {\n" +
						"\t\treturn new " + iface.name + "(binder);\n\n" +
						"\t}\n\n";
				
				foreach (AIDLMethod method in iface.methods) {
					code += GenerateMethod(method);
				}
				
				code += "}\n\n";
				return code;
			}
			
			public static string GenerateMethod(AIDLMethod method) {
				string methodString = "";
				
				// Method signature
				methodString += "\tpublic ";
				methodString += WriteType(method.returnType) + " ";
				methodString += UppercaseFirst(method.name) + "(";
				
				// Method parameters
				if (method.parameters.Count > 0) {
					foreach(AIDLParam par in method.parameters) {
						methodString += WriteType(par.ParamType) + " " + par.name + ", ";
					}
					
					methodString = methodString.Remove(methodString.LastIndexOf(","));
				}
				
				methodString += ") {\n";
				
				// Return type
				if (method.returnType != typeof(void)) {
					methodString += "\t\t" + WriteType(method.returnType) + " _result =";
					
					if (method.returnType.IsPrimitive) {
						methodString += " 0;\n\n";
					} else if (method.returnType == typeof(string)) {
						methodString += " \"\";\n\n";
					} else {
						methodString += " null;\n\n";
					}
				}
				// Handle IPC behavior
				methodString += "\t\tif (UseProxy) {\n";
				methodString += "\t\t\tAndroidJavaObject _data = CreateParcel();\n";
				methodString += "\t\t\tAndroidJavaObject _reply = CreateParcel();\n\n";
				methodString += "\t\t\ttry {\n";
				methodString += "\t\t\t\t_data.Call(\"writeInterfaceToken\", Descriptor);\n";
				
				// Pass parameters to parcel
				foreach (AIDLParam par in method.parameters) {
					methodString += AddTypeToRequest(par);
				}
				methodString += "\n";
				
				// Call remote
				methodString += "\t\t\t\tmJavaObject.Call<bool>(\"transact\", TRANSACTION_" + method.name + ", _data, _reply, 0);\n";
				
				// Check for exception
				methodString += "\t\t\t\t_reply.Call(\"readException\");\n\n";
				
				// Parse return
				methodString += WriteReturnType(method);
				
				// Release Parcel objects
				methodString += "\t\t\t} finally {\n";
				methodString += "\t\t\t\t_data.Call(\"recycle\");\n";
				methodString += "\t\t\t\t_reply.Call(\"recycle\");\n";
				methodString += "\t\t\t}\n\n";
				
				
				// Handle direct method call
				methodString += "\t\t} else {\n";
				if (method.returnType != typeof(void)) {
					methodString += "\t\t\treturn mJavaObject.Call<" + WriteType(method.returnType) + ">(\"" + method.name + "\", ";
				} else {
					methodString += "\t\t\tmJavaObject.Call(\"" + method.name + "\", ";
				}
				
				// Pass parameters to direct method call
				foreach (AIDLParam par in method.parameters) {
					methodString += par.name + ", ";
				}
				methodString = methodString.Remove(methodString.LastIndexOf(","));
				methodString += ");\n";
				methodString += "\t\t}\n\n";
				
				// Return result (if exists)
				if (method.returnType != typeof(void)) {
					methodString += "\t\treturn _result;\n";
				}
				
				methodString += "\t}\n\n";
				
				return methodString;
			}
			
			private static string AddTypeToRequest(AIDLParam parameter) {
				// Parse C# type
				if (parameter.ParamType == typeof(string)) {
					return "\t\t\t\t_data.Call(\"writeString\", " + parameter.name + ");\n";
					
				} else if (parameter.ParamType == typeof(bool)) {
					return "\t\t\t\t_data.Call(\"writeBoolean\", " + parameter.name + ");\n";
					
				} else if (parameter.ParamType == typeof(byte)) {
					return "\t\t\t\t_data.Call(\"writeByte\", " + parameter.name + ");\n";
					
				} else if (parameter.ParamType == typeof(char)) {
					return "\t\t\t\t_data.Call(\"writeChar\", " + parameter.name + ");\n";
					
				} else if (parameter.ParamType == typeof(short)) {
					return "\t\t\t\t_data.Call(\"writeShort\", " + parameter.name + ");\n";
					
				} else if (parameter.ParamType == typeof(int)) {
					return "\t\t\t\t_data.Call(\"writeInt\", " + parameter.name + ");\n";
					
				} else if (parameter.ParamType == typeof(long)) {
					return "\t\t\t\t_data.Call(\"writeLong\", " + parameter.name + ");\n";
					
				} else if (parameter.ParamType == typeof(float)) {
					return "\t\t\t\t_data.Call(\"writeFloat\", " + parameter.name + ");\n";
					
				} else if (parameter.ParamType == typeof(double)) {
					return "\t\t\t\t_data.Call(\"writeDouble\", " + parameter.name + ");\n";
					
				} else {
					if (parameter.typeName == "android.os.Bundle") {
						return "\t\t\t\tif (" + parameter.name + " != null) {\n" +
							   "\t\t\t\t\t_data.Call(\"writeInt\", 1);\n" +
							   "\t\t\t\t\t" + parameter.name + ".Call(\"writeToParcel\", _data, 0);\n" +
							   "\t\t\t\t} else {\n" +
							   "\t\t\t\t\t_data.Call(\"writeInt\", 0);\n" +
							   "\t\t\t\t}\n";
					} else {
						
					}					
				}
				
				return "";
			}
			
			private static string WriteReturnType(AIDLMethod method) {
				// Parse C# type
				if (method.returnType == typeof(string)) {
					return "\t\t\t\t_result = _reply.Call<string>(\"readString\");\n\n";
					
				} else if (method.returnType == typeof(bool)) {
					return "\t\t\t\t_result = _reply.Call<bool>(\"readBoolean\");\n\n";
					
				} else if (method.returnType == typeof(byte)) {
					return "\t\t\t\t_result = _reply.Call<byte>(\"readByte\");\n\n";
					
				} else if (method.returnType == typeof(char)) {
					return "\t\t\t\t_result = _reply.Call<char>(\"readChar\");\n\n";
					
				} else if (method.returnType == typeof(short)) {
					return "\t\t\t\t_result = _reply.Call<short>(\"readShort\");\n\n";
					
				} else if (method.returnType == typeof(int)) {
					return "\t\t\t\t_result = _reply.Call<int>(\"readInt\");\n\n";
					
				} else if (method.returnType == typeof(long)) {
					return "\t\t\t\t_result = _reply.Call<long>(\"readLong\");\n\n";
					
				} else if (method.returnType == typeof(float)) {
					return "\t\t\t\t_result = _reply.Call<float>(\"readFloat\");\n\n";
					
				} else if (method.returnType == typeof(double)) {
					return "\t\t\t\t_result = _reply.Call<double>(\"readDouble\");\n\n";
					
				} else if (method.returnType == typeof(AndroidJavaObject)) {
					if (method.returnTypeName == "android.os.Bundle") {
						return 	"\t\t\t\tif (0 !=_reply.Call<int>(\"readInt\")) {\n" +
								//"\t\t\t\t\t_result = new AndroidJavaClass(\"android.os.Bundle\").Get<AndroidJavaObject>(\"CREATOR\").Call<AndroidJavaObject>(\"createFromParcel\", _reply);\n" +
								"\t\t\t\t\tusing(AndroidJavaClass klazz = new AndroidJavaClass(\"android.os.Bundle\")) {\n" +
								"\t\t\t\t\t\tAndroidJavaObject CREATOR = klazz.GetStatic<AndroidJavaObject>(\"CREATOR\");\n" +
								"\t\t\t\t\t\t_result = CREATOR.Call<AndroidJavaObject>(\"createFromParcel\", _reply);\n" +
								"\t\t\t\t\t}\n"	 +
								"\t\t\t\t} else {\n" +
								"\t\t\t\t\t_result = null;\n" +
								"\t\t\t\t}\n\n";
					}
				} else {
					
				}				
				
				return "";
			}
			
			private static string WriteType(Type paramType) {
				// Parse C# type
				if (paramType == typeof(string)) {
					return "string";
					
				} else if (paramType == typeof(void)) {
					return "void";
					
				} else if (paramType == typeof(bool)) {
					return "bool";
					
				} else if (paramType == typeof(byte)) {
					return "byte";
					
				} else if (paramType == typeof(char)) {
					return "char";
					
				} else if (paramType == typeof(short)) {
					return "short";
					
				} else if (paramType == typeof(int)) {
					return "int";
					
				} else if (paramType == typeof(long)) {
					return "long";
					
				} else if (paramType == typeof(float)) {
					return "float";
					
				} else if (paramType == typeof(double)) {
					return "double";
					
				} else {
					// TODO for Unity, this is AndroidJavaObject
					return "AndroidJavaObject";
					
				}
			}
				
			private static string UppercaseFirst(string s) {
				if (string.IsNullOrEmpty(s)) {
				    return string.Empty;
				}
				
				char[] a = s.ToCharArray();
				a[0] = char.ToUpper(a[0]);
				return new string(a);
			}
		}
		
	}
}

