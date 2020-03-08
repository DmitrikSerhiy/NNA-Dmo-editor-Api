export enum RightMenues {
    dashboard = 'dashboard',
    dmoCollections = 'dmoCollections',
    dmos = 'dmos'
}

export enum LeftMenuTabs {
    dashboard = 'dashboard',
    dmoCollections = 'dmoCollections',
    dmos = 'dmos'
}

export class DmoCollectionShortDto {
    id: string;
    dmoCount: number;
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

export class DmoShorterDto {
    constructor(id: string, movieTitle: string, dmoName: string) {
        this.id = id;
        this.movieTitle = movieTitle;
        this.name = dmoName;
    }
    id: string;
    movieTitle: string;
    name: string;
}

export class ShortDmoCollectionDto {
    collectionName: string;
    dmos: DmoShorterDto[];
}

export class DmoCollectionDto {
    id: string;
    collectionName: string;
    dmos: DmoShortDto[];
}

export class DmosIdDto {
    constructor(id: string) {
        this.id = id;
    }
    id: string;
}

export class AddDmosToCollectionDto {
    collectionId: string;
    dmos: DmosIdDto[];
}

