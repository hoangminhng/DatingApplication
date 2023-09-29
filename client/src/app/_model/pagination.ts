export interface Pagination{
  currentPage: number;
  itemPerpage: number;
  totalItems: number;
  totalPages:number;
}

export class PaginationResult<T>{
  result?: T;
  pagination?: Pagination;
}
