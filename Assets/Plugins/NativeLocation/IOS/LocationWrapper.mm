//
//  MIT License
//
//  Copyright (c) 2018 Daniel Lupiañez Casares
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//

#import "LocationWrapper.h"
#import <CoreLocation/CoreLocation.h>

@interface LocationWrapper () <CLLocationManagerDelegate>
@property (nonatomic, assign) LocationReceivedDelegate locationReceivedCallback;
@property (nonatomic, assign) LocationErrorDelegate locationErrorCallback;
@property (nonatomic, strong) CLLocationManager *locationManager;
@property (nonatomic, assign) BOOL isUsingDeferredLocationUpdates;
@end

@implementation LocationWrapper

#pragma mark - Static

+ (instancetype) sharedWrapper
{
    static dispatch_once_t sharedLocationWrapperToken;
    static LocationWrapper *sharedLocationWrapper;
    
    dispatch_once (&sharedLocationWrapperToken, ^{
        sharedLocationWrapper = [[LocationWrapper alloc] init];
    });
    
    return sharedLocationWrapper;
}

#pragma mark - Initializers

- (instancetype) init
{
    self = [super init];
    if (self)
    {
        [self setLocationManager:[[CLLocationManager alloc] init]];
        [self setIsUsingDeferredLocationUpdates:NO];
    }
    return self;
}

#pragma mark - Public methods

- (LocationWrapperAuthorizationStatus) deviceAuthorizationStatus
{
    if ([CLLocationManager locationServicesEnabled])
        return LocationWrapperAuthorizationStatusAuthorized;
    return LocationWrapperAuthorizationStatusDenied;
}

- (LocationWrapperAuthorizationStatus) applicationAuthorizationStatus
{
    switch ([CLLocationManager authorizationStatus]) {
        case kCLAuthorizationStatusAuthorizedAlways:
        case kCLAuthorizationStatusAuthorizedWhenInUse:
            return LocationWrapperAuthorizationStatusAuthorized;
            break;
        case kCLAuthorizationStatusDenied:
        case kCLAuthorizationStatusRestricted:
            return LocationWrapperAuthorizationStatusDenied;
            break;
        case kCLAuthorizationStatusNotDetermined:
            return LocationWrapperAuthorizationStatusNotDetermined;
            break;
    }
    return LocationWrapperAuthorizationStatusDenied;
}

- (void) requestApplicationPermissions
{
    [[self locationManager] requestWhenInUseAuthorization];
}

- (void) goToApplicationSettings
{
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
}

- (void) start
{
    [[self locationManager] setDelegate:self];
    [[self locationManager] setPausesLocationUpdatesAutomatically:YES];
    [[self locationManager] setActivityType:CLActivityTypeFitness];
    [[self locationManager] setDistanceFilter:10];
    [[self locationManager] setDesiredAccuracy:kCLLocationAccuracyBest];
    [[self locationManager] setAllowsBackgroundLocationUpdates:YES];
    [[self locationManager] startUpdatingLocation];
}

- (void) stop
{
    [[self locationManager] stopUpdatingLocation];
}

#pragma mark - CLLocationManager Delegate

- (void) locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray<CLLocation *> *)locations
{
    for (CLLocation *location in locations)
        self.locationReceivedCallback(location.coordinate.latitude, location.coordinate.longitude, location.altitude, 0.0);
    
    if ([CLLocationManager deferredLocationUpdatesAvailable])
    {
        [manager allowDeferredLocationUpdatesUntilTraveled:500 timeout:10];
        [self setIsUsingDeferredLocationUpdates:YES];
    }
}

- (void) locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error
{
    self.locationErrorCallback([[error localizedDescription] UTF8String], [error code]);
}

- (void) locationManager:(CLLocationManager *)manager didFinishDeferredUpdatesWithError:(NSError *)error
{
    self.locationErrorCallback([[error localizedDescription] UTF8String], [error code]);
    [self setIsUsingDeferredLocationUpdates:NO];
}

#pragma mark - C# Method View

extern "C"
{
    int IOSLocationWrapperGetDeviceAuthorizationStatus()
    {
        return [[LocationWrapper sharedWrapper] deviceAuthorizationStatus];
    }
    
    int IOSLocationWrapperGetApplicationAuthorizationStatus()
    {
        return [[LocationWrapper sharedWrapper] applicationAuthorizationStatus];
    }
    
    void IOSLocationWrapperRequestApplicationPermissions()
    {
        [[LocationWrapper sharedWrapper] requestApplicationPermissions];
    }
    
    void IOSLocationWrapperGoToApplicationSettings()
    {
        [[LocationWrapper sharedWrapper] goToApplicationSettings];
    }
    
    void IOSLocationWrapperStartTrackingLocation(LocationReceivedDelegate locationCallback, LocationErrorDelegate errorCallback)
    {
        [[LocationWrapper sharedWrapper] setLocationReceivedCallback:locationCallback];
        [[LocationWrapper sharedWrapper] setLocationErrorCallback:errorCallback];
        [[LocationWrapper sharedWrapper] start];
    }
    
    void IOSLocationWrapperStopTrackingLocation()
    {
        [[LocationWrapper sharedWrapper] stop];
        [[LocationWrapper sharedWrapper] setLocationReceivedCallback:nil];
        [[LocationWrapper sharedWrapper] setLocationErrorCallback:nil];
    }
}

@end

