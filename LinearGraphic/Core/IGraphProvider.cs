using Model;
using System;

namespace Core;

public interface IGraphProvider
{
    object GetGraphControl();
    void Initialize(GraphSettings settings);
    void UpdatePoints(Point[] points);
}
