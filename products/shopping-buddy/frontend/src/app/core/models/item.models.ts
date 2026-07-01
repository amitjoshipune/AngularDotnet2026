export interface Item {
  id: number;
  name: string;
  category: string;
  status: string;
  createdAt: string;
}

export interface ItemCreateRequest {
  name: string;
  category: string;
  status: string;
}

export interface ItemUpdateRequest extends ItemCreateRequest {
  id: number;
}
