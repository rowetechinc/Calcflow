/*
 * 由SharpDevelop创建。
 * 用户： ggg
 * 日期: 2011-12-20
 * 时间: 16:27
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace EstimatingFlowMode
{
	/// <summary>
	/// 三点外延
	/// </summary>
	public class Slope
	{
        /// <summary>
        /// 计算三点外延的类
        /// </summary>
		public Slope()
		{
		}
		// ax + by + c = 0
		private double a = 0;
		private double b = 0;
		private double c = 0;
		/// <summary>
		/// 源数据设置
		/// </summary>
		/// <param name="Xs">点集的X坐标集合</param>
		/// <param name="Ys">点集的Y坐标集合</param>
		public  void setXYs(double[] Xs, double[] Ys)
		{
			if(Xs.Length != Ys.Length || Xs.Length == 0)
			{
				throw new ArgumentException("Xs or Ys counts shoud be equal and not zero!");
			}
			
			double sumXY = 0.0;
			double sumX = 0.0;
			double sumY = 0.0;
			double sumXX = 0.0;
			int n = Xs.Length;
			
			double x = 0.0;
			double y = 0.0;
			double slope = 0.0;
			double _b = 0.0;
			
			for (int i = 0; i < n; i++)
			{
				sumX += Xs[i];
				sumY += Ys[i];
				sumXX += Xs[i] * Xs[i];
				sumXY += Xs[i] * Ys[i];
			}
			
			x = sumX / n;
			y = sumY / n;
			
			double cal_base = n * sumXX  - sumX * sumX;
			if(cal_base == 0.0)
			{
				b = 0;
				a = 1;
				c = -x;
				return;
			}
			
			slope = (n * sumXY - sumX * sumY)/cal_base;
			_b = (sumY * sumXX - sumX * sumXY )/cal_base;
			
			b = 1;
			a = -slope;
			c = - _b;
			return;
		}
		/// <summary>
		/// 根据指定Y值得到X
		/// </summary>
		/// <param name="y">Y</param>
		/// <returns>X</returns>
		public double calX(double y)
		{
			if(a == 0)
			{
				throw new DivideByZeroException("y allways be a constant![ a = 0 in (ax + by + c = 0) ]");
			}
			else
			{
				double x;
				x = 0 -(b*y+c)/a;
				return x;
			}
		}
		/// <summary>
		/// 根据指定X计算Y
		/// </summary>
		/// <param name="x">X</param>
		/// <returns>Y</returns>
		public double calY(double x)
		{
			if(b == 0)
			{
				throw new DivideByZeroException("x allways be a constant![ b = 0 in (ax + by + c = 0) ]");
			}
			else
			{
				double y;
				y = 0 -(a*x+c)/b;
				return y;
			}
		}
	}
}
