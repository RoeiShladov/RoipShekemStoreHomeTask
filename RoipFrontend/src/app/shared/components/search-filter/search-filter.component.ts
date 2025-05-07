import { Component, OnInit, Output, EventEmitter, Input, OnDestroy, NgModule } from '@angular/core';
import { Subject, Subscription, debounceTime, distinctUntilChanged } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-search-filter',
  standalone: true,
  templateUrl: './search-filter.component.html',
  styleUrls: ['./search-filter.component.scss'],
  imports: [MatCardModule, CommonModule, FormsModule, MatIconModule, BrowserAnimationsModule, MatButtonModule, MatPaginatorModule, MatTableModule, MatInputModule, MatFormFieldModule],
})

export class SearchFilterComponent implements OnInit, OnDestroy {
  @Input() placeholder: string = 'Search...';
  @Output() searchChange = new EventEmitter<string>();

  searchQuery = '';
  private searchSubject = new Subject<string>();
  private searchSubscription: Subscription | undefined;

  constructor() { }

  ngOnInit(): void {
    this.setupSearchListener();
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  setupSearchListener(): void {
    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(300), // Wait 300ms after each keystroke
      distinctUntilChanged() // Only emit if the current value is different than the last
    ).subscribe(query => {
      this.searchChange.emit(query);
    });
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchQuery);
  }
}
