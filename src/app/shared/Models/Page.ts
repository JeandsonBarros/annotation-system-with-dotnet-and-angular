export interface Page<T> {
    data: T;
    page: number;
    size: number;
    totalPages: number;
    totalRecords: number;
    nextPage: string;
    previousPage: string;
    firstPage: string;
    lastPage: string;
}