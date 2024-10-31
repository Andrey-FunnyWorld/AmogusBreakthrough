using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridPaging : MonoBehaviour {
    public int PageSize = 10;
    public Transform PrevPageItem, NextPageItem;
    int pageIndex = -1;
    int totalCount = 0;
    int maxPageIndex = 0;
    Transform[] allItems, currentPageItems;
    public int PageIndex {
        get { return pageIndex; }
        set {
            if (pageIndex != value && value >= 0 && value <= maxPageIndex) {
                pageIndex = value;
                PageIndexChanged(pageIndex);
            }
        }
    }
    void PageIndexChanged(int newPageIndex) {
        PrevPageItem.gameObject.SetActive(false);
        NextPageItem.gameObject.SetActive(false);
        PrevPageItem.SetParent(null);
        NextPageItem.SetParent(null);
        if (currentPageItems != null)
            foreach (Transform item in currentPageItems) {
                item.gameObject.SetActive(false);
                item.transform.SetParent(null);
            }
        if (newPageIndex > 0) {
            PrevPageItem.SetParent(transform);
            PrevPageItem.gameObject.SetActive(true);
        }
        currentPageItems = GetPageItems(newPageIndex);
        foreach (Transform item in currentPageItems) {
            item.gameObject.SetActive(true);
            item.transform.SetParent(transform);
            item.transform.localScale = Vector2.one;
        }
        if (newPageIndex < maxPageIndex) {
            NextPageItem.SetParent(transform);
            NextPageItem.gameObject.SetActive(true);
        }
        PrevPageItem.transform.localScale = Vector2.one;
        NextPageItem.transform.localScale = Vector2.one;
    }
    Transform[] GetPageItems(int pageIndex) {
        int skip = pageIndex * PageSize + (pageIndex > 0 ? 1 : 0);
        int take = pageIndex == 0 ? PageSize + 1 : PageSize;
        return allItems.Skip(skip).Take(take).ToArray();
    }
    public void SetPageSize(int newPageSize) {
        pageIndex = -1;
        PageSize = newPageSize;
        CalcPageDate(newPageSize);
        PageIndex = 0;
    }
    void CalcPageDate(int pageSize) {
        maxPageIndex = (int)Mathf.Ceil((float)totalCount / pageSize) - 1;
        if (totalCount % pageSize == 1) maxPageIndex--;
    }
    public void SetAllItems(Transform[] items) {
        allItems = items;
        totalCount = allItems.Length;
        CalcPageDate(PageSize);
        PageIndex = 0;
    }
    public void NextPage() {
        PageIndex = PageIndex + 1;
    }
    public void PrevPage() {
        PageIndex = PageIndex - 1;
    }
}
