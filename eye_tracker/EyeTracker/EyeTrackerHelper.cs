using System;
using System.Drawing;

class EyeTrackerHelper {
    public EyeTrackerHelper(int _faceWidthCal, int _webcamWidth, int _webcamHeight) {
        if (faceWidthCal == 0) 
            faceWidthCal = _faceWidthCal;
        if (webcamWidth == 0)
            webcamWidth = _webcamWidth;
        if (webcamHeight == 0)
            webcamHeight = _webcamHeight;
        if (ratioBase == 0)
            ratioBase = faceWidthCal/webcamWidth;
    }

    private double getDistanceToFace(double faceWidth) {
        // TODO: This may be the incorrect calculation
        return distanceToFaceCal*(faceWidth/faceWidthCal);
    }

    public void setVirtualEyeDepth(double _virtualEyeDepth) {
        virtualEyeDepth = _virtualEyeDepth;
    }

    public Point getPupilRelativePos(int eyeWidth, int eyeHeight, int faceWidth, int eyeCenterRelativeWebcamX, int eyeCenterRelativeWebcamY) {
        Point eye = new Point();
        double eyeRelativeCenterX = eyeWidth/2;
        double eyeRelativeCenterY = eyeHeight/2;
        double webcamCenterX = webcamWidth/2;
        double webcamCenterY = webcamHeight/2;
        double depth = getDistanceToFace(faceWidth);
        eye.X = Convert.ToInt32(eyeRelativeCenterX + (virtualEyeDepth * (eyeCenterRelativeWebcamX-webcamCenterX) / depth));
        eye.Y = Convert.ToInt32(eyeRelativeCenterY + (virtualEyeDepth * (eyeCenterRelativeWebcamY-webcamCenterY) / depth));
        return eye;
    }

    public double getDistanceToFaceCal() {
        return distanceToFaceCal;
    }

    private static double ratioBase = 0;
    private static int faceWidthCal = 0;
    private static int webcamWidth = 0;
    private static int webcamHeight = 0;
    private const double distanceToFaceCal = 0.7;
    private static double virtualEyeDepth = 0;
}