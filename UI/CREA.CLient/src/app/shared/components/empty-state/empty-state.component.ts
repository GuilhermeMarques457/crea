import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [MatIconModule],
  templateUrl: `./empty-state.component.html`,
})
export class EmptyStateComponent {
  icon = input<string>('inbox');
  message = input.required<string>();
  description = input<string>();
}
