﻿namespace thehomebrewapi.ResourceParameters
{
    public abstract class BaseResourceParameters
    {
        const int MAX_PAGE_SIZE = 50;

        public string SearchQuery { get; set; }
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }

        public string OrderBy { get; set; } = "Name";
    }
}
