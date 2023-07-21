import { AnnotationDto } from "../DTOs/AnnotationDto";

export interface AnnotationUpdateParameters {
    annotationId: number;
    annotationDto: AnnotationDto;
  }