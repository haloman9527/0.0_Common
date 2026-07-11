using System;
using UnityEditor;
using UnityEngine;

namespace Atom.UnityEditors.IMGUI.Controls
{
    /// <summary>
    /// 虚拟列表数据源。列表只请求当前可见行，因此适合大量数据。
    /// </summary>
    public interface IVirtualListDataSource
    {
        /// <summary>列表中的数据数量。</summary>
        int Count { get; }

        /// <summary>获取用于显示的行文本。</summary>
        string GetDisplayName(int index);

        /// <summary>判断行是否被选中。</summary>
        bool IsSelected(int index);

        /// <summary>更新选中行。</summary>
        void SetSelected(int index);
    }

    /// <summary>
    /// 轻量 IMGUI 虚拟列表。只绘制可见行，不构建 TreeViewItem 树。
    /// </summary>
    public sealed class VirtualListView
    {
        private const float ScrollbarWidth = 16.0f;

        public float RowHeight { get; set; } = 24.0f;
        public float ScrollPosition { get; set; }
        public Action<int> ItemDoubleClicked;

        private int lastMaxBeginIndex;
        private int lastFullVisibleRowCount;
        private float lastScrollRange;

        /// <summary>
        /// 绘制列表并处理鼠标和键盘选择。
        /// </summary>
        public void OnGUI(Rect rect, IVirtualListDataSource dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            var count = dataSource.Count;
            var contentRect = new Rect(rect.x, rect.y, Mathf.Max(1.0f, rect.width - ScrollbarWidth), rect.height);
            var fullVisibleRowCount = Mathf.Max(1, Mathf.FloorToInt(contentRect.height / RowHeight));
            var drawRowCount = Mathf.Max(1, Mathf.CeilToInt(contentRect.height / RowHeight));
            var scrollbarSize = count > 0 ? Mathf.Clamp01((float)fullVisibleRowCount / count) : 1.0f;
            var scrollRange = Mathf.Max(0.0f, 1.0f - scrollbarSize);
            var maxBeginIndex = Mathf.Max(0, count - fullVisibleRowCount);
            lastMaxBeginIndex = maxBeginIndex;
            lastFullVisibleRowCount = fullVisibleRowCount;
            lastScrollRange = scrollRange;
            HandleMouseWheel(rect, maxBeginIndex, scrollRange);

            var normalizedScroll = scrollRange > 0.0f ? ScrollPosition / scrollRange : 0.0f;
            var beginIndex = Mathf.Clamp(Mathf.RoundToInt(normalizedScroll * maxBeginIndex), 0, maxBeginIndex);
            var drawCount = Mathf.Min(drawRowCount, count - beginIndex);

            HandleKeyboard(rect, dataSource, maxBeginIndex, fullVisibleRowCount, scrollRange);

            GUI.BeginClip(contentRect);
            for (var i = 0; i < drawCount; i++)
            {
                var index = beginIndex + i;
                var rowRect = new Rect(0.0f, i * RowHeight, contentRect.width, RowHeight);
                var selected = dataSource.IsSelected(index);

                if (GUI.Toggle(rowRect, selected, dataSource.GetDisplayName(index), EditorStyles.toolbarButton) != selected)
                {
                    dataSource.SetSelected(index);
                    GUI.FocusControl(null);
                }

                if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && rowRect.Contains(Event.current.mousePosition))
                {
                    ItemDoubleClicked?.Invoke(index);
                    Event.current.Use();
                }
            }
            GUI.EndClip();

            if (count == 0)
            {
                EditorGUI.LabelField(contentRect, "没有可显示的数据。", EditorStyles.centeredGreyMiniLabel);
            }

            var scrollbarRect = new Rect(rect.xMax - ScrollbarWidth, rect.y, ScrollbarWidth, rect.height);
            ScrollPosition = GUI.VerticalScrollbar(scrollbarRect, ScrollPosition, scrollbarSize, 0.0f, 1.0f);
        }

        /// <summary>
        /// 将指定索引完整定位到可视区域。应在列表至少绘制一次后调用。
        /// </summary>
        public void FrameItem(int itemIndex)
        {
            SetScrollToItem(itemIndex, lastMaxBeginIndex, lastFullVisibleRowCount, lastScrollRange);
        }

        /// <summary>
        /// 定位数据源中的当前选中项。
        /// </summary>
        public void FrameSelected(IVirtualListDataSource dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            FrameItem(GetSelectedIndex(dataSource));
        }

        /// <summary>
        /// 将鼠标滚轮增量转换为行索引变化，再映射回滚动条取值范围。
        /// </summary>
        private void HandleMouseWheel(Rect rect, int maxBeginIndex, float scrollRange)
        {
            if (maxBeginIndex <= 0 || Event.current.type != EventType.ScrollWheel || !rect.Contains(Event.current.mousePosition))
            {
                return;
            }

            var normalizedScroll = scrollRange > 0.0f ? ScrollPosition / scrollRange : 0.0f;
            var currentBeginIndex = Mathf.RoundToInt(normalizedScroll * maxBeginIndex);
            var rowDelta = Mathf.RoundToInt(Event.current.delta.y * 3.0f);

            if (rowDelta == 0)
            {
                rowDelta = Event.current.delta.y > 0.0f ? 1 : -1;
            }

            var nextBeginIndex = Mathf.Clamp(currentBeginIndex + rowDelta, 0, maxBeginIndex);
            ScrollPosition = (float)nextBeginIndex / maxBeginIndex * scrollRange;
            Event.current.Use();
        }

        /// <summary>
        /// 处理上下方向键，保持选择行位于列表可视区域。
        /// </summary>
        private void HandleKeyboard(
            Rect rect,
            IVirtualListDataSource dataSource,
            int maxBeginIndex,
            int visibleRowCount,
            float scrollRange)
        {
            if (Event.current.type != EventType.KeyDown || !rect.Contains(Event.current.mousePosition))
            {
                return;
            }

            var selectedIndex = GetSelectedIndex(dataSource);
            if (Event.current.keyCode == KeyCode.DownArrow)
            {
                var nextIndex = Mathf.Min(selectedIndex + 1, dataSource.Count - 1);
                dataSource.SetSelected(nextIndex);
                SetScrollToItem(nextIndex, maxBeginIndex, visibleRowCount, scrollRange);
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.UpArrow)
            {
                var nextIndex = Mathf.Max(selectedIndex - 1, 0);
                dataSource.SetSelected(nextIndex);
                SetScrollToItem(nextIndex, maxBeginIndex, visibleRowCount, scrollRange);
                Event.current.Use();
            }
        }

        /// <summary>
        /// 将指定行定位到可视区域内；目标行已可见时不改变滚动位置。
        /// </summary>
        private void SetScrollToItem(int itemIndex, int maxBeginIndex, int visibleRowCount, float scrollRange)
        {
            if (maxBeginIndex <= 0)
            {
                return;
            }

            var normalizedScroll = scrollRange > 0.0f ? ScrollPosition / scrollRange : 0.0f;
            var beginIndex = Mathf.RoundToInt(normalizedScroll * maxBeginIndex);
            var endIndex = beginIndex + visibleRowCount - 1;

            if (itemIndex < beginIndex)
            {
                beginIndex = itemIndex;
            }
            else if (itemIndex > endIndex)
            {
                beginIndex = itemIndex - visibleRowCount + 1;
            }
            else
            {
                return;
            }

            beginIndex = Mathf.Clamp(beginIndex, 0, maxBeginIndex);
            ScrollPosition = (float)beginIndex / maxBeginIndex * scrollRange;
        }

        private int GetSelectedIndex(IVirtualListDataSource dataSource)
        {
            for (var i = 0; i < dataSource.Count; i++)
            {
                if (dataSource.IsSelected(i))
                {
                    return i;
                }
            }

            return 0;
        }
    }
}
