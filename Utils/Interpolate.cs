using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// From: http://wiki.unity3d.com/index.php/Interpolate
// Original JS version: Fernando Zapata (fernando@cpudreams.com)
// C# Version: Andrea85cs (andrea85cs@dynematica.it)
// added conveinence functions for t in [0,1] - Nolan Baker
 
namespace Paraphernalia.Utils {
public class Interpolate {
    
 /**
 * Different methods of easing interpolation.
 */
    public enum EaseType {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InSine,
        OutSine,
        InOutSine,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc
    }
 
    /**
    * Sequence of eleapsedTimes until elapsedTime is >= duration.
    *
    * Note: elapsedTimes are calculated using the value of Time.deltatTime each
    * time a value is requested.
    */
    public static Vector3 Identity(Vector3 v) {
        return v;
    }
 
    public static Vector3 TransformDotPosition(Transform t) {
        return t.position;
    }
 
 
    public static IEnumerable<float> NewTimer(float duration) {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration) {
            yield return elapsedTime;
            elapsedTime += Time.deltaTime;
            // make sure last value is never skipped
            if (elapsedTime >= duration) {
                yield return elapsedTime;
            }
        }
    }
 
    public delegate Vector3 ToVector3<T>(T v);
    public delegate float Function(float a, float b, float c, float d);
 
    /**
     * Generates sequence of integers from start to end (inclusive) one step
     * at a time.
     */
    public static IEnumerable<float> NewCounter(int start, int end, int step) {
        for (int i = start; i <= end; i += step) {
            yield return i;
        }
    }
 
    /**
     * Returns sequence generator from start to end over duration using the
     * given easing function. The sequence is generated as it is accessed
     * using the Time.deltaTime to calculate the portion of duration that has
     * elapsed.
     */
    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float duration) {
        IEnumerable<float> timer = Interpolate.NewTimer(duration);
        return NewEase(ease, start, end, duration, timer);
    }
 
    /**
     * Instead of easing based on time, generate n interpolated points (slices)
     * between the start and end positions.
     */
    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, int slices) {
        IEnumerable<float> counter = Interpolate.NewCounter(0, slices + 1, 1);
        return NewEase(ease, start, end, slices + 1, counter);
    }
 
 
 
    /**
     * Generic easing sequence generator used to implement the time and
     * slice variants. Normally you would not use this function directly.
     */
    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float total, IEnumerable<float> driver) {
        Vector3 distance = end - start;
        foreach (float i in driver) {
            yield return Ease(ease, start, distance, i, total);
        }
    }
 
    /**
     * Vector3 interpolation using given easing method. Easing is done independently
     * on all three vector axis.
     */
    public static Vector3 Ease(Function ease, Vector3 start, Vector3 distance, float elapsedTime, float duration) {
        start.x = ease(start.x, distance.x, elapsedTime, duration);
        start.y = ease(start.y, distance.y, elapsedTime, duration);
        start.z = ease(start.z, distance.z, elapsedTime, duration);
        return start;
    }
 
    /**
     * Returns the public static method that implements the given easing type for scalars.
     * Use this method to easily switch between easing interpolation types.
     *
     * All easing methods clamp elapsedTime so that it is always <= duration.
     *
     * var ease = Interpolate.Ease(EaseType.EaseInQuad);
     * i = ease(start, distance, elapsedTime, duration);
     */
    public static Function Ease(EaseType type) {
        // Source Flash easing functions:
        // http://gizma.com/easing/
        // http://www.robertpenner.com/easing/easing_demo.html
        //
        // Changed to use more friendly variable names, that follow my Lerp
        // conventions:
        // start = b (start value)
        // distance = c (change in value)
        // elapsedTime = t (current time)
        // duration = d (time duration)
 
        Function f = null;
        switch (type) {
            case EaseType.Linear: f = Interpolate.Linear; break;
            case EaseType.InQuad: f = Interpolate.EaseInQuad; break;
            case EaseType.OutQuad: f = Interpolate.EaseOutQuad; break;
            case EaseType.InOutQuad: f = Interpolate.EaseInOutQuad; break;
            case EaseType.InCubic: f = Interpolate.EaseInCubic; break;
            case EaseType.OutCubic: f = Interpolate.EaseOutCubic; break;
            case EaseType.InOutCubic: f = Interpolate.EaseInOutCubic; break;
            case EaseType.InQuart: f = Interpolate.EaseInQuart; break;
            case EaseType.OutQuart: f = Interpolate.EaseOutQuart; break;
            case EaseType.InOutQuart: f = Interpolate.EaseInOutQuart; break;
            case EaseType.InQuint: f = Interpolate.EaseInQuint; break;
            case EaseType.OutQuint: f = Interpolate.EaseOutQuint; break;
            case EaseType.InOutQuint: f = Interpolate.EaseInOutQuint; break;
            case EaseType.InSine: f = Interpolate.EaseInSine; break;
            case EaseType.OutSine: f = Interpolate.EaseOutSine; break;
            case EaseType.InOutSine: f = Interpolate.EaseInOutSine; break;
            case EaseType.InExpo: f = Interpolate.EaseInExpo; break;
            case EaseType.OutExpo: f = Interpolate.EaseOutExpo; break;
            case EaseType.InOutExpo: f = Interpolate.EaseInOutExpo; break;
            case EaseType.InCirc: f = Interpolate.EaseInCirc; break;
            case EaseType.OutCirc: f = Interpolate.EaseOutCirc; break;
            case EaseType.InOutCirc: f = Interpolate.EaseInOutCirc; break;
        }
        return f;
    }

    public static float Ease(EaseType type, float t) {
        switch (type) {
            case EaseType.Linear: return Interpolate.Linear(t);
            case EaseType.InQuad: return Interpolate.EaseInQuad(t);
            case EaseType.OutQuad: return Interpolate.EaseOutQuad(t);
            case EaseType.InOutQuad: return Interpolate.EaseInOutQuad(t);
            case EaseType.InCubic: return Interpolate.EaseInCubic(t);
            case EaseType.OutCubic: return Interpolate.EaseOutCubic(t);
            case EaseType.InOutCubic: return Interpolate.EaseInOutCubic(t);
            case EaseType.InQuart: return Interpolate.EaseInQuart(t);
            case EaseType.OutQuart: return Interpolate.EaseOutQuart(t);
            case EaseType.InOutQuart: return Interpolate.EaseInOutQuart(t);
            case EaseType.InQuint: return Interpolate.EaseInQuint(t);
            case EaseType.OutQuint: return Interpolate.EaseOutQuint(t);
            case EaseType.InOutQuint: return Interpolate.EaseInOutQuint(t);
            case EaseType.InSine: return Interpolate.EaseInSine(t);
            case EaseType.OutSine: return Interpolate.EaseOutSine(t);
            case EaseType.InOutSine: return Interpolate.EaseInOutSine(t);
            case EaseType.InExpo: return Interpolate.EaseInExpo(t);
            case EaseType.OutExpo: return Interpolate.EaseOutExpo(t);
            case EaseType.InOutExpo: return Interpolate.EaseInOutExpo(t);
            case EaseType.InCirc: return Interpolate.EaseInCirc(t);
            case EaseType.OutCirc: return Interpolate.EaseOutCirc(t);
            case EaseType.InOutCirc: return Interpolate.EaseInOutCirc(t);
            default: return t;
        }
    }
 
    /**
     * Returns sequence generator from the first node to the last node over
     * duration time using the points in-between the first and last node
     * as control points of a bezier curve used to generate the interpolated points
     * in the sequence. If there are no control points (ie. only two nodes, first
     * and last) then this behaves exactly the same as NewEase(). In other words
     * a zero-degree bezier spline curve is just the easing method. The sequence
     * is generated as it is accessed using the Time.deltaTime to calculate the
     * portion of duration that has elapsed.
     */
    public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, float duration) {
        IEnumerable<float> timer = Interpolate.NewTimer(duration);
        return NewBezier<Transform>(ease, nodes, TransformDotPosition, duration, timer);
    }
 
    /**
     * Instead of interpolating based on time, generate n interpolated points
     * (slices) between the first and last node.
     */
    public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, int slices) {
        IEnumerable<float> counter = NewCounter(0, slices + 1, 1);
        return NewBezier<Transform>(ease, nodes, TransformDotPosition, slices + 1, counter);
    }
 
    /**
     * A Vector3[] variation of the Transform[] NewBezier() function.
     * Same functionality but using Vector3s to define bezier curve.
     */
    public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, float duration) {
        IEnumerable<float> timer = NewTimer(duration);
        return NewBezier<Vector3>(ease, points, Identity, duration, timer);
    }
 
    /**
     * A Vector3[] variation of the Transform[] NewBezier() function.
     * Same functionality but using Vector3s to define bezier curve.
     */
    public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, int slices) {
        IEnumerable<float> counter = NewCounter(0, slices + 1, 1);
        return NewBezier<Vector3>(ease, points, Identity, slices + 1, counter);
    }
 
    /**
     * Generic bezier spline sequence generator used to implement the time and
     * slice variants. Normally you would not use this function directly.
     */
    public static IEnumerable<Vector3> NewBezier<T>(Function ease, IList nodes, ToVector3<T> toVector3, float maxStep, IEnumerable<float> steps) {
        // need at least two nodes to spline between
        if (nodes.Count >= 2) {
            // copy nodes array since Bezier is destructive
            Vector3[] points = new Vector3[nodes.Count];
 
            foreach (float step in steps) {
                // re-initialize copy before each destructive call to Bezier
                for (int i = 0; i < nodes.Count; i++) {
                    points[i] = toVector3((T)nodes[i]);
                }
                yield return Bezier(ease, points, step, maxStep);
                // make sure last value is always generated
            }
        }
    }
 
    /**
     * A Vector3 n-degree bezier spline.
     *
     * WARNING: The points array is modified by Bezier. See NewBezier() for a
     * safe and user friendly alternative.
     *
     * You can pass zero control points, just the start and end points, for just
     * plain easing. In other words a zero-degree bezier spline curve is just the
     * easing method.
     *
     * @param points start point, n control points, end point
     */
    public static Vector3 Bezier(Function ease, Vector3[] points, float elapsedTime, float duration) {
        // Reference: http://ibiblio.org/e-notes/Splines/Bezier.htm
        // Interpolate the n starting points to generate the next j = (n - 1) points,
        // then interpolate those n - 1 points to generate the next n - 2 points,
        // continue this until we have generated the last point (n - (n - 1)), j = 1.
        // We store the next set of output points in the same array as the
        // input points used to generate them. This works because we store the
        // result in the slot of the input point that is no longer used for this
        // iteration.
        for (int j = points.Length - 1; j > 0; j--) {
            for (int i = 0; i < j; i++) {
                points[i].x = ease(points[i].x, points[i + 1].x - points[i].x, elapsedTime, duration);
                points[i].y = ease(points[i].y, points[i + 1].y - points[i].y, elapsedTime, duration);
                points[i].z = ease(points[i].z, points[i + 1].z - points[i].z, elapsedTime, duration);
            }
        }
        return points[0];
    } 

    public static Vector3 QuadBezier(Vector3 start, Vector3 cp, Vector3 end, float t) {
        float f = (1-t);
        return f*f*start + 2*f*t*cp + t*t*end;
    }

    public static Vector3 CubicBezier(Vector3 start, Vector3 cp1, Vector3 cp2, Vector3 end, float t) {
        float f = (1-t);
        return f*f*f*start + 3*f*f*t*cp1 + 3*f*t*t*cp2 + t*t*t*end;
    }
 
    /**
     * Returns sequence generator from the first node, through each control point,
     * and to the last node. N points are generated between each node (slices)
     * using Catmull-Rom.
     */
    public static IEnumerable<Vector3> NewCatmullRom(Transform[] nodes, int slices, bool loop) {
        return NewCatmullRom<Transform>(nodes, TransformDotPosition, slices, loop);
    }
 
    /**
     * A Vector3[] variation of the Transform[] NewCatmullRom() function.
     * Same functionality but using Vector3s to define curve.
     */
    public static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, int slices, bool loop) {
        return NewCatmullRom<Vector3>(points, Identity, slices, loop);
    }
 
    /**
     * Generic catmull-rom spline sequence generator used to implement the
     * Vector3[] and Transform[] variants. Normally you would not use this
     * function directly.
     */
    public static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, ToVector3<T> toVector3, int slices, bool loop) {
        // need at least two nodes to spline between
        if (nodes.Count >= 2) {
 
            // yield the first point explicitly, if looping the first point
            // will be generated again in the step for loop when interpolating
            // from last point back to the first point
            yield return toVector3((T)nodes[0]);
 
            int last = nodes.Count - 1;
            for (int current = 0; loop || current < last; current++) {
                // wrap around when looping
                if (loop && current > last) {
                    current = 0;
                }
                // handle edge cases for looping and non-looping scenarios
                // when looping we wrap around, when not looping use start for previous
                // and end for next when you at the ends of the nodes array
                int previous = (current == 0) ? ((loop) ? last : current) : current - 1;
                int start = current;
                int end = (current == last) ? ((loop) ? 0 : current) : current + 1;
                int next = (end == last) ? ((loop) ? 0 : end) : end + 1;
 
                // adding one guarantees yielding at least the end point
                int stepCount = slices + 1;
                for (int step = 1; step <= stepCount; step++) {
                    yield return CatmullRom(toVector3((T)nodes[previous]),
                                     toVector3((T)nodes[start]),
                                     toVector3((T)nodes[end]),
                                     toVector3((T)nodes[next]),
                                     step, stepCount);
                }
            }
        }
    }
 
    /**
     * A Vector3 Catmull-Rom spline. Catmull-Rom splines are similar to bezier
     * splines but have the useful property that the generated curve will go
     * through each of the control points.
     *
     * NOTE: The NewCatmullRom() functions are an easier to use alternative to this
     * raw Catmull-Rom implementation.
     *
     * @param previous the point just before the start point or the start point
     *                 itself if no previous point is available
     * @param start generated when elapsedTime == 0
     * @param end generated when elapsedTime >= duration
     * @param next the point just after the end point or the end point itself if no
     *             next point is available
     */
    public static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float elapsedTime, float duration) {
    	return CatmullRom(previous, start, end, next, elapsedTime / duration);
    }

    public static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float t) {
        // References used:
        // p.266 GemsV1
        //
        // tension is often set to 0.5 but you can use any reasonable value:
        // http://www.cs.cmu.edu/~462/projects/assn2/assn2/catmullRom.pdf
        //
        // bias and tension controls:
        // http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/interpolation/
 
        float percentComplete = t;
        float percentCompleteSquared = percentComplete * percentComplete;
        float percentCompleteCubed = percentCompleteSquared * percentComplete;
 
        return previous * (-0.5f * percentCompleteCubed +
                                   percentCompleteSquared -
                            0.5f * percentComplete) +
                start   * ( 1.5f * percentCompleteCubed +
                           -2.5f * percentCompleteSquared + 1.0f) +
                end     * (-1.5f * percentCompleteCubed +
                            2.0f * percentCompleteSquared +
                            0.5f * percentComplete) +
                next    * ( 0.5f * percentCompleteCubed -
                            0.5f * percentCompleteSquared);
    }
 
 
 
 
    /**
     * Linear interpolation (same as Mathf.Lerp)
     */
    public static float Linear(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        elapsedTime /= duration;
        return Linear(start, distance, elapsedTime);
    }

    public static float Linear(float start, float distance, float t) {
        return distance * t + start;
    }

    public static float Linear(float t) {
        return t;
    }
 
    /**
     * quadratic easing in - accelerating from zero velocity
     */
    public static float EaseInQuad(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return EaseInQuad(start, distance, elapsedTime);
    }

    public static float EaseInQuad(float start, float distance, float t) {
        return distance * t * t + start;
    }

    public static float EaseInQuad(float t) {
        return t * t;
    }
 
    /**
     * quadratic easing out - decelerating to zero velocity
     */
    public static float EaseOutQuad(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return EaseOutQuad(start, distance, elapsedTime);
    }

    public static float EaseOutQuad(float start, float distance, float t) {
        return -distance * t * (t - 2) + start;
    }

    public static float EaseOutQuad(float t) {
        return -t * (t - 2);
    }
 
    /**
     * quadratic easing in/out - acceleration until halfway, then deceleration
     */
    public static float EaseInOutQuad(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime + start;
        elapsedTime--;
        return EaseInOutQuad(start, distance, elapsedTime);
    }

    public static float EaseInOutQuad(float start, float distance, float t) {
        return -distance / 2 * (t * (t - 2) - 1) + start;
    }

    public static float EaseInOutQuad(float t) {
        if (t < 0.5f) return EaseInQuad(2 * t) * 0.5f;
        else return EaseOutQuad(2 * (t - 0.5f)) * 0.5f + 0.5f;
    }
 
    /**
     * cubic easing in - accelerating from zero velocity
     */
    public static float EaseInCubic(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return EaseInCubic(start, distance, elapsedTime);
    }

    public static float EaseInCubic(float start, float distance, float t) {
        return distance * t * t * t + start;
    }

    public static float EaseInCubic(float t) {
        return t * t * t;
    }
 
    /**
     * cubic easing out - decelerating to zero velocity
     */
    public static float EaseOutCubic(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return EaseOutCubic(start, distance, elapsedTime);
    }

    public static float EaseOutCubic(float start, float distance, float t) {
        return distance * (t * t * t + 1) + start;
    }

    public static float EaseOutCubic(float t) {
        t = 1 - t;
        return 1 - t * t * t;
    }
 
    /**
     * cubic easing in/out - acceleration until halfway, then deceleration
     */
    public static float EaseInOutCubic(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return EaseInOutCubic(start, distance, elapsedTime);
    }

    public static float EaseInOutCubic(float start, float distance, float t) {
        return distance / 2 * (t * t * t + 2) + start;
    }

    public static float EaseInOutCubic(float t) {
        if (t < 0.5f) return EaseInCubic(2 * t) * 0.5f;
        else return EaseOutCubic(2 * (t - 0.5f)) * 0.5f + 0.5f;
    }
 
    /**
     * quartic easing in - accelerating from zero velocity
     */
    public static float EaseInQuart(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return EaseInQuart(start, distance, elapsedTime);
    }

    public static float EaseInQuart(float start, float distance, float t) {
        return distance * t * t * t * t + start;
    }

    public static float EaseInQuart(float t) {
        return t * t * t * t;
    }
 
    /**
     * quartic easing out - decelerating to zero velocity
     */
    public static float EaseOutQuart(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return EaseOutQuart(start, distance, elapsedTime);
    }

    public static float EaseOutQuart(float start, float distance, float t) {
        return -distance * (t * t * t * t - 1) + start;
    }

    public static float EaseOutQuart(float t) {
        t = 1 - t;
        return 1 - t * t * t * t;
    }
 
    /**
     * quartic easing in/out - acceleration until halfway, then deceleration
     */
    public static float EaseInOutQuart(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return EaseInOutQuart(start, distance, elapsedTime);
    }

    public static float EaseInOutQuart(float start, float distance, float t) {
        if (t < 0.5f) return EaseInQuart(start, distance, t);
       	else return EaseOutQuart(start, distance, t);
    }

    public static float EaseInOutQuart(float t) {
        if (t < 0.5f) return EaseInQuart(t);
        else return EaseOutQuart(t);
    }
 

    /**
     * quintic easing in - accelerating from zero velocity
     */
    public static float EaseInQuint(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return EaseInQuint(start, distance, elapsedTime);
    }

    public static float EaseInQuint(float start, float distance, float t) {
        return distance * t * t * t * t * t + start;
    }

    public static float EaseInQuint(float t) {
        return t * t * t * t * t;
    }
 
    /**
     * quintic easing out - decelerating to zero velocity
     */
    public static float EaseOutQuint(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return EaseOutQuint(start, distance, elapsedTime);
    }

    public static float EaseOutQuint(float start, float distance, float t) {
        return distance * (t * t * t * t * t + 1) + start;
    }

    public static float EaseOutQuint(float t) {
        t = 1 - t;
        return 1 - t * t * t * t * t;
    }
 
    /**
     * quintic easing in/out - acceleration until halfway, then deceleration
     */
    public static float EaseInOutQuint(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return EaseInOutQuint(start, distance, elapsedTime);
    }

    public static float EaseInOutQuint(float start, float distance, float t) {
        if (t < 0.5f) return EaseInQuint(start, distance, t);
        else return EaseOutQuint(start, distance, t);
    }

    public static float EaseInOutQuint(float t) {
        if (t < 0.5f) return EaseInQuint(t);
        else return EaseOutQuint(t);
    }
 
    /**
     * sinusoidal easing in - accelerating from zero velocity
     */
    public static float EaseInSine(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        elapsedTime /= duration;
        return EaseInSine(start, distance, elapsedTime);
    }
 
    public static float EaseInSine(float start, float distance, float t) {
        return -distance * Mathf.Cos(t * (Mathf.PI / 2)) + distance + start;
    }
 
    public static float EaseInSine(float t) {
        return -Mathf.Cos(t * (Mathf.PI / 2)) + 1;
    }
 
    /**
     * sinusoidal easing out - decelerating to zero velocity
     */
    public static float EaseOutSine(float start, float distance, float elapsedTime, float duration) {
        if (elapsedTime > duration) { elapsedTime = duration; }
        elapsedTime /= duration;
        return EaseOutSine(start, distance, elapsedTime);
    }

    public static float EaseOutSine(float start, float distance, float t) {
        return distance * Mathf.Sin(t * (Mathf.PI / 2)) + start;
    }

    public static float EaseOutSine(float t) {
        return Mathf.Sin(t * (Mathf.PI / 2));
    }
 
    /**
     * sinusoidal easing in/out - accelerating until halfway, then decelerating
     */
    public static float EaseInOutSine(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        elapsedTime /= duration;
        return EaseInOutSine(start, distance, elapsedTime);
    }
 
    public static float EaseInOutSine(float start, float distance, float t) {
        return -distance / 2 * (Mathf.Cos(Mathf.PI * t) - 1) + start;
    }
 
    public static float EaseInOutSine(float t) {
        return -0.5f * (Mathf.Cos(Mathf.PI * t) - 1);
    }
 
    /**
     * exponential easing in - accelerating from zero velocity
     */
    public static float EaseInExpo(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        elapsedTime /= duration;
        return EaseInExpo(start, distance, elapsedTime);
    }
 
    public static float EaseInExpo(float start, float distance, float t) {
        return distance * Mathf.Pow(2, 10 * (t - 1)) + start;
    }
 
    public static float EaseInExpo(float t) {
        return Mathf.Pow(2, 10 * (t - 1));
    }

    /**
     * exponential easing out - decelerating to zero velocity
     */
    public static float EaseOutExpo(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
		elapsedTime /= duration;
        return EaseOutExpo(start, distance, elapsedTime);
    }

    public static float EaseOutExpo(float start, float distance, float t) {
        return distance * (-Mathf.Pow(2, -10 * t) + 1) + start;
    }

    public static float EaseOutExpo(float t) {
        return -Mathf.Pow(2, -10 * t) + 1;
    }
 
    /**
     * exponential easing in/out - accelerating until halfway, then decelerating
     */
    public static float EaseInOutExpo(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 *  Mathf.Pow(2, 10 * (elapsedTime - 1)) + start;
        elapsedTime--;
        return EaseInOutExpo(start, distance, elapsedTime);
    }

    public static float EaseInOutExpo(float start, float distance, float t) {
        return distance / 2 * (-Mathf.Pow(2, -10 * t) + 2) + start;
    }

    public static float EaseInOutExpo(float t) {
        if (t < 0.5f) return EaseInExpo(2 * t) * 0.5f;
        else return EaseOutExpo(2 * (t - 0.5f)) * 0.5f + 0.5f;
    }
 
    /**
     * circular easing in - accelerating from zero velocity
     */
    public static float EaseInCirc(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return EaseInCirc(start, distance, elapsedTime);
    }

    public static float EaseInCirc(float start, float distance, float t) {
        return -distance * (Mathf.Sqrt(1 - t * t) - 1) + start;
    }

    public static float EaseInCirc(float t) {
        return -Mathf.Sqrt(1 - t * t) + 1;
    }
 
    /**
     * circular easing out - decelerating to zero velocity
     */
    public static float EaseOutCirc(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return EaseOutCirc(start, distance, elapsedTime);
    }

    public static float EaseOutCirc(float start, float distance, float t) {
        return distance * Mathf.Sqrt(1 - t * t) + start;
    }

    public static float EaseOutCirc(float t) {
        t -= 1;
        return Mathf.Sqrt(1 - t * t);
    }
 
    /**
     * circular easing in/out - acceleration until halfway, then deceleration
     */
    public static float EaseInOutCirc(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return -distance / 2 * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start;
        elapsedTime -= 2;
        return EaseInOutCirc(start, distance, elapsedTime);
    }

    public static float EaseInOutCirc(float start, float distance, float t) {
        return distance / 2 * (Mathf.Sqrt(1 - t * t) + 1) + start;
    }

    public static float EaseInOutCirc(float t) {
        if (t < 0.5f) return EaseInCirc(2 * t) * 0.5f;
        else return EaseOutCirc(2 * (t - 0.5f)) * 0.5f + 0.5f;
    }
}
}