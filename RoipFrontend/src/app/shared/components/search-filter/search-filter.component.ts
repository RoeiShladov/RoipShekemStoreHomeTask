import { Component, OnInit, Output, EventEmitter, Input, OnDestroy } from '@angular/core';
import { Subject, Subscription, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-search-filter',
  standalone: false, // Ensure this is marked as standalone  
  templateUrl: './search-filter.component.html',
  styleUrls: ['./search-filter.component.scss']
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
