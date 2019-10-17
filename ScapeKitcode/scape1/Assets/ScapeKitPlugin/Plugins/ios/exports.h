
#pragma once 

#if _WIN32
  #define STDCALL __stdcall
  #define EXPORT_DLL __declspec( dllexport ) 
#else 
  #define STDCALL
  #define EXPORT_DLL
#endif