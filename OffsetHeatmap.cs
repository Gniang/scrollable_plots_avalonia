using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avalonia_play
{
    /// <summary>
    /// 表示範囲、表示位置を指定できるScottPlotのヒートマップ（WinForms用）
    /// </summary>
    public class OffsetHeatmap
    {
        private readonly IEnumerable<Heatmap> heatmaps;
        private double xOffset = 0;
        private double xMin;
        private double xZoom = 0;

        public Double XOffsetMeter => xOffset / Constants.PIXEL_PER_METER;
        public Double XZoomMeter => xZoom / Constants.PIXEL_PER_METER;

        public AvaPlot View { get; }

        public OffsetHeatmap(AvaPlot plot, IEnumerable<Heatmap> heatmaps)
        {
            this.heatmaps = heatmaps;
            this.View = plot;
        }

        /// <summary>
        /// x座標のオフセット量を設定
        /// </summary>
        public OffsetHeatmap SetXOffset(double xOffset)
        {
            double oldXOffset = this.xOffset;
            this.xOffset = xOffset;

            double changedOffset = this.xOffset - oldXOffset;

            foreach (var hm in heatmaps)
            {
                hm.XMin += changedOffset;
                hm.XMax += changedOffset;
            }
            return this;
        }

        /// <summary>
        /// Plotコントロールを再描画する
        /// </summary>
        public OffsetHeatmap Refresh()
        {
            this.View.Refresh();
            return this;
        }

        /// <summary>
        /// X軸の表示範囲を指定する
        /// </summary>
        /// <param name="xCord"></param>
        public OffsetHeatmap SetXZoom(double xCord)
        {
            this.xZoom = xCord;
            ResetAxisLimits();
            return this;
        }

        /// <summary>
        /// X軸の開始位置を指定する
        /// </summary>
        /// <param name="xCord"></param>
        public OffsetHeatmap SetXMin(double xCord)
        {
            this.xMin = xCord;
            ResetAxisLimits();
            return this;
        }

        /// <summary>
        /// 表示範囲を指定する
        /// </summary>
        private void ResetAxisLimits()
        {
            if (this.xZoom == 0)
            {
                return;
            }

            this.View.Plot.SetAxisLimits(
                xMin: this.xMin,
                xMax: this.xMin + this.xZoom,
                yMin: 0,
                yMax: Constants.IMG_SIZE.Height);
        }
    }

}
