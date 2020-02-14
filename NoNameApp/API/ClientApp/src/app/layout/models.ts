export enum RightMenues {
    dmoCollections = 'dmoCollections',
    dmos = 'dmos'
}

export class DmoCollectionShortDto {
    id: string;
    collectionName: string;
}

export class DmoShortDto {
    id: string;
    name: string;
    movieTitle: string;
    dmoStatus: string;
    dmoStatusId: number;
    shortComment: string;
    mark: number;
}

export class DmoCollectionDto {
    id: string;
    collectionName: string;
    dmos: DmoShortDto[];
}

