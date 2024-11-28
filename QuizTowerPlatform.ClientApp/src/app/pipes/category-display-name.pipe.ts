import { Pipe, PipeTransform } from '@angular/core';
import { QUIZ_GENRES } from '../shared/quiz-genres.constant';

@Pipe({
  name: 'categoryDisplayName'
})
export class CategoryDisplayNamePipe implements PipeTransform {
  private categoryNames = QUIZ_GENRES;

  transform(value: number): string {
    const genre = this.categoryNames.find(genre => genre.value === value);
    return genre ? genre.name : 'Unknown';
  }

}
