import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ItemService } from '../../core/services/item.service';
import { Item } from '../../core/models/item.models';

@Component({
  selector: 'app-items',
  templateUrl: './items.component.html',
  styleUrls: ['./items.component.css'],
})
export class ItemsComponent implements OnInit {
  items: Item[] = [];
  searchTerm = '';
  editingId: number | null = null;
  form: FormGroup;
  isSubmitting = false;
  message = '';

  constructor(private readonly itemService: ItemService, private readonly fb: FormBuilder) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      category: ['', Validators.required],
      status: ['Active', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems(): void {
    this.itemService.getItems(this.searchTerm).subscribe({
      next: (items) => (this.items = items),
      error: () => (this.message = 'Unable to load items right now.'),
    });
  }

  search(): void {
    this.loadItems();
  }

  startEdit(item: Item): void {
    this.editingId = item.id;
    this.form.patchValue({
      name: item.name,
      category: item.category,
      status: item.status,
    });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.form.reset({ status: 'Active' });
  }

  submit(): void {
    if (this.form.invalid) {
      return;
    }

    this.isSubmitting = true;
    const value = this.form.value;

    const request = {
      name: value.name,
      category: value.category,
      status: value.status,
    };

    const action = this.editingId
      ? this.itemService.updateItem({ ...request, id: this.editingId })
      : this.itemService.createItem(request);

    action.subscribe({
      next: () => {
        this.isSubmitting = false;
        this.message = this.editingId ? 'Item updated.' : 'Item created.';
        this.cancelEdit();
        this.loadItems();
      },
      error: () => {
        this.isSubmitting = false;
        this.message = 'The request could not be completed.';
      },
    });
  }

  deleteItem(id: number): void {
    this.itemService.deleteItem(id).subscribe({
      next: () => {
        this.message = 'Item deleted.';
        this.loadItems();
      },
      error: () => (this.message = 'Delete failed.'),
    });
  }
}
