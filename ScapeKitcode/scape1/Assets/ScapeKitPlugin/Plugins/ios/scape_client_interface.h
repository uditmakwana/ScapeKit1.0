
#pragma once

#include "exports.h"

extern "C" {

  typedef void(STDCALL *onScapeMeasurementsRequestedCB)(int timestamp);
  typedef void(STDCALL *onScapeSessionErrorCB)(int errorStatus, const char* errorMessage);
  
  struct scape_measurements {
      double timestamp; 
      double latitude; double longitude; double heading; 
      double orientationX; 
      double orientationY; 
      double orientationZ; 
      double orientationW; 
      double rawHeightEstimate; 
      double confidenceScore; 
      int measurementsStatus;
  };

  typedef void(STDCALL *onScapeMeasurementsUpdatedCB)(scape_measurements sm);

  struct motion_measurements {
    double acceleration[3];
    double accelerationTimeStamp;
    double userAcceleration[3];
    double gyro[3];
    double gyroTimestamp;
    double magnetometer[3];
    double magnetometerTimestamp;
    double gravity[3];
    double attitude[3];
  };

  typedef void(STDCALL *onAquireMotionMeasurementsCB)(motion_measurements& mm);

  struct location_measurements {
    double timestamp;
    double latitude;
    double longitude;
    double coordinatesAccuracy;
    double altitude;
    double altitudeAccuracy;
    double heading;
    double headingAccuracy;
    int64_t course;
    int64_t speed;
  };

  typedef void(STDCALL *onAquireLocationMeasurementsCB)(location_measurements& lm);

  const int SmallStringSize = 256;

  struct device_info {
    char id[SmallStringSize];
    char platform[SmallStringSize];
    char model[SmallStringSize];
    char os[SmallStringSize];
    char os_version[SmallStringSize];
    char api_version[SmallStringSize];
    char write_directory[SmallStringSize];
    char sdk_version[SmallStringSize];
  };

  EXPORT_DLL void* citf_createClient(const char* api_key, int with_debug);

  EXPORT_DLL void* citf_getDebugSession(void* client);

  EXPORT_DLL void* citf_getGlobalScapeClient();

  EXPORT_DLL void citf_setLogConfig(void* debug, int log_level, int log_output);
  EXPORT_DLL void citf_mockGPSCoordinates(void* debug, double latitude, double longitude);
  EXPORT_DLL void citf_saveImages(void* debug, bool save);

  EXPORT_DLL void citf_setSessionCallbacks( void* client,
                        onScapeMeasurementsRequestedCB req,
                        onScapeSessionErrorCB err,
                        onScapeMeasurementsUpdatedCB mes );

  EXPORT_DLL void citf_getMeasurements(void* client);
  EXPORT_DLL void citf_setYChannelPtr(void* client, int64_t pointer, int32_t width, int32_t height);
  EXPORT_DLL void citf_setCameraIntrinsics(void* client, double xFocalLength, double yFocalLength, double xPrincipalPoint, double yPrincipalPoint);
        
  EXPORT_DLL void citf_setDeviceInfo(void* client, const device_info&);

  EXPORT_DLL void citf_setClientStateCallbacks( void* client, 
                        onAquireMotionMeasurementsCB motion, 
                        onAquireLocationMeasurementsCB loc);

  EXPORT_DLL void citf_destroyClient(void* client);
  
  EXPORT_DLL void citf_log(int logLevel, const char* tag, const char*  msg);
}
